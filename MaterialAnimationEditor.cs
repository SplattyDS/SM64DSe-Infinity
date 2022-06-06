using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SM64DSe.ImportExport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SM64DSe.ImportExport.Loaders.InternalLoaders;
using SM64DSe.SM64DSFormats;
using System.Text.RegularExpressions;
using SM64DSe.ImportExport.Writers.InternalWriters;
using System.Drawing.Imaging;
using System.IO;

namespace SM64DSe
{
    public partial class MaterialAnimationEditor : Form
    {
        protected class ModelImportationSettings
        {
            public float m_Scale;
            public float m_InGamePreviewScale;
            public BMDImporter.BMDExtraImportOptions m_ExtraOptions;

            public ModelImportationSettings(float scale, float gameScale, BMDImporter.BMDExtraImportOptions extraOptions)
            {
                m_Scale = scale;
                m_InGamePreviewScale = gameScale;
                m_ExtraOptions = extraOptions;
            }

            public ModelImportationSettings(float gameScale)
                : this(1f, gameScale, BMDImporter.BMDExtraImportOptions.DEFAULT) { }

            public ModelImportationSettings()
                : this(1f) { }

            public float GetImportationScale()
            {
                return m_Scale;
            }

            public float GetPreviewScale()
            {
                return m_Scale * m_InGamePreviewScale;
            }
        }

        private ModelBase m_ModelBase;

        private SortedDictionary<string, ModelBase.TextureDefNitro> m_WorkingTexturesCopy;
        private SortedDictionary<string, byte[]> m_WorkingPalettesCopy;

        private string m_Name;
        private bool m_ModelSourceLoaded;

        private BMD m_LoadedModel;
        private BMA m_LoadedMatAnim;
        private BMA.MaterialProperties m_SelectedMatProp;

        private ModelImportationSettings m_ModelImportationSettings;

        private float m_ModelPreviewScale;

        private int[] m_BMDDisplayLists;

        private Timer m_AnimationTimer;
        private int m_AnimationFrameNumber = 0;
        private int m_AnimationNumFrames = -1;
        private bool m_LoopAnimation = true;
        private bool m_Running = false;

        private FolderBrowserDialog m_FolderBrowserDialogue;
        private ROMFileSelect m_ROMFileSelect = new ROMFileSelect();

        private static readonly byte[] DUMMY_BMD_DATA;
        private static readonly NitroFile DUMMY_BMD_NITRO_FILE;
        private static readonly BMD DUMMY_BMD;
        static MaterialAnimationEditor()
        {
            DUMMY_BMD_DATA = new byte[0x30];
            DUMMY_BMD_NITRO_FILE = new NitroFile();
            DUMMY_BMD_NITRO_FILE.m_ID = 0xFFFF;
            DUMMY_BMD_NITRO_FILE.m_Name = "DUMMY_BMD";
            DUMMY_BMD_NITRO_FILE.m_Data = DUMMY_BMD_DATA;
            DUMMY_BMD = new BMD(DUMMY_BMD_NITRO_FILE);
        }

        public MaterialAnimationEditor(
            float gameScale = 1f
            )
        {
            m_BMDDisplayLists = new int[3]; // Standard, Geometry Highlighting, Skeleton

            m_ModelImportationSettings = new ModelImportationSettings(gameScale);

            m_ModelPreviewScale = m_ModelImportationSettings.GetPreviewScale();

            m_ModelSourceLoaded = false;

            InitialiseForm();
            InitTimer();
        }

        private void InitialiseForm()
        {
            InitializeComponent();

            UpdateEnabledStateMenuControls();

            // Model GL
            glModelView.Initialise();
            glModelView.ProvideDisplayLists(m_BMDDisplayLists);
            glModelView.ProvideScaleRef(ref m_ModelPreviewScale);

            // File selection dialogues
            m_FolderBrowserDialogue = new FolderBrowserDialog();
            m_FolderBrowserDialogue.SelectedPath = System.IO.Path.GetDirectoryName(Program.m_ROMPath);
        }

        private void MaterialAnimationEditor_Load(object sender, System.EventArgs e)
        {
            
        }

        private void MaterialAnimationEditor_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (!m_ModelSourceLoaded) return;

            glModelView.PrepareForClose();

            if (m_LoadedModel != null) m_LoadedModel.Release();
        }

        private void LoadModel()
        {
            m_ModelBase = null;

            if (m_Name != null && m_Name != "" && m_Name.EndsWith(".bmd"))
            {
                try
                {
                    m_ModelBase = BMDImporter.LoadModel(m_Name);
                }
                catch (Exception e)
                {
                    new ExceptionMessageBox("Error loading model", e).ShowDialog();
                    return;
                }
            }

            DeleteDisplayLists();

            // BMD
            m_LoadedModel = DUMMY_BMD;

            m_WorkingTexturesCopy = new SortedDictionary<string, ModelBase.TextureDefNitro>();
            m_WorkingPalettesCopy = new SortedDictionary<string, byte[]>();

            foreach (KeyValuePair<string, ModelBase.TextureDefBase> textureEntry in m_ModelBase.m_Textures)
            {
                NitroTexture nitroTexture = BMDWriter.ConvertTexture(0, 0, textureEntry.Value,
                    m_ModelImportationSettings.m_ExtraOptions.m_TextureQualitySetting, false);
                ModelBase.TextureDefNitro textureDefNitroTexture = new ModelBase.TextureDefNitro(nitroTexture);

                m_WorkingTexturesCopy.Add(textureDefNitroTexture.GetTexName(), textureDefNitroTexture);

                if (textureDefNitroTexture.HasNitroPalette())
                {
                    byte[] palette = textureDefNitroTexture.GetNitroPalette();
                    byte[] paletteCopy = new byte[palette.Length];
                    Array.Copy(palette, paletteCopy, palette.Length);
                    string key = textureDefNitroTexture.GetPalName();

                    if (!m_WorkingPalettesCopy.ContainsKey(key))
                        m_WorkingPalettesCopy.Add(key, paletteCopy);
                }

                foreach (ModelBase.MaterialDef material in m_ModelBase.m_Materials.Values)
                {
                    if (textureEntry.Key.Equals(material.m_TextureDefID))
                    {
                        material.m_TextureDefID = textureDefNitroTexture.GetTexName();
                    }
                }
            }

            m_ModelBase.m_Textures.Clear();

            txtModelPreviewScale.BackColor = Color.White;

            m_ModelSourceLoaded = true;

            UpdateBMDModelAndPreview();

            // General
            UpdateEnabledStateMenuControls();

            lblMainStatus.Text = "Source: " + m_Name;
        }

        private void DeleteDisplayLists()
        {
            foreach (int dl in m_BMDDisplayLists)
            {
                if (dl > 0) GL.DeleteLists(dl, 1);
            }
        }

        private void PrerenderBMDModel()
        {
            if (m_BMDDisplayLists[0] == 0)
            {
                m_BMDDisplayLists[0] = GL.GenLists(1);
            }
            GL.NewList(m_BMDDisplayLists[0], ListMode.Compile);
            GL.PushAttrib(AttribMask.AllAttribBits);

            Vector3 previewScale = new Vector3(m_ModelPreviewScale);

            if (m_ModelSourceLoaded)
            {
                GL.Enable(EnableCap.Lighting);
                GL.Light(LightName.Light0, LightParameter.Position, new Vector4(1.0f, 1.0f, 1.0f, 0.0f));
                GL.Light(LightName.Light0, LightParameter.Ambient, Color.White);
                GL.Light(LightName.Light0, LightParameter.Diffuse, Color.White);
                GL.Light(LightName.Light0, LightParameter.Specular, Color.White);
                GL.PushMatrix();
                GL.Scale(previewScale);
                GL.FrontFace(FrontFaceDirection.Ccw);

                m_LoadedModel.PrepareToRender();

                m_LoadedModel.Render(RenderMode.Opaque, 1f);
                m_LoadedModel.Render(RenderMode.Translucent, 1f);

                GL.PopMatrix();
            }
            GL.PopAttrib();
            GL.EndList();
        }

        private void UpdateBMDModelAndPreview()
        {
            m_ModelBase.m_Textures.Clear();
            foreach (KeyValuePair<string, ModelBase.TextureDefNitro> textureEntry in m_WorkingTexturesCopy)
            {
                m_ModelBase.m_Textures.Add(textureEntry.Key, textureEntry.Value);
            }

            m_LoadedModel = CallBMDImporter(false);

            glModelView.SetShowMarioReference(true);
            PrerenderBMDModel();
            glModelView.Refresh();
        }

        private BMD CallBMDImporter(bool save = false)
        {
            Dictionary<string, Dictionary<string, ModelBase.GeometryDef>> originalGeometries =
                new Dictionary<string, Dictionary<string, ModelBase.GeometryDef>>();
            foreach (ModelBase.BoneDef bone in m_ModelBase.m_BoneTree)
            {
                Dictionary<string, ModelBase.GeometryDef> boneGeometries = new Dictionary<string, ModelBase.GeometryDef>();
                foreach (KeyValuePair<string, ModelBase.GeometryDef> geometryEntry in bone.m_Geometries)
                {
                    boneGeometries[geometryEntry.Key] = DuplicateGeometry(geometryEntry.Value);
                }
                originalGeometries[bone.m_ID] = boneGeometries;
            }

            m_ModelBase.ScaleModel(m_ModelImportationSettings.GetImportationScale());

            BMD result = BMDImporter.CallBMDWriter(ref m_LoadedModel.m_File,
                m_ModelBase, m_ModelImportationSettings.m_ExtraOptions, save);

            foreach (ModelBase.BoneDef bone in m_ModelBase.m_BoneTree)
            {
                foreach (ModelBase.GeometryDef geometry in bone.m_Geometries.Values)
                {
                    foreach (ModelBase.PolyListDef polyList in geometry.m_PolyLists.Values)
                    {
                        polyList.m_FaceLists =
                            originalGeometries[bone.m_ID][geometry.m_ID].m_PolyLists[polyList.m_ID].m_FaceLists;
                    }
                }
            }

            return result;
        }

        private void RefreshBMDScale()
        {
            m_ModelPreviewScale = m_ModelImportationSettings.GetPreviewScale();
            if (m_ModelSourceLoaded)
            {
                PrerenderBMDModel();
                glModelView.Refresh();
            }
        }

        private void ResetColourButtonValue(Button button)
        {
            button.Text = null;
            button.BackColor = Color.Transparent;
            button.ForeColor = Color.Black;
        }

        private void UpdateEnabledStateMenuControls()
        {
            mnitLoad.Enabled = true;

            mnitImport.Enabled = m_ModelSourceLoaded;

            mnitExport.Enabled = m_ModelSourceLoaded;
        }

        private ModelBase.PolyListDef DuplicatePolylist(ModelBase.PolyListDef sourcePolyList)
        {
            ModelBase.PolyListDef polyList = new ModelBase.PolyListDef(sourcePolyList.m_ID, sourcePolyList.m_MaterialName);
            foreach (ModelBase.FaceListDef sourceFaceList in sourcePolyList.m_FaceLists)
            {
                ModelBase.FaceListDef faceList = new ModelBase.FaceListDef(sourceFaceList.m_Type);
                foreach (ModelBase.FaceDef sourceFace in sourceFaceList.m_Faces)
                {
                    ModelBase.FaceDef face = new ModelBase.FaceDef(sourceFace.m_NumVertices);
                    for (int i = 0; i < sourceFace.m_NumVertices; i++)
                    {
                        face.m_Vertices[i] = sourceFace.m_Vertices[i];
                    }
                    faceList.m_Faces.Add(face);
                }
                polyList.m_FaceLists.Add(faceList);
            }

            return polyList;
        }

        private ModelBase.GeometryDef DuplicateGeometry(ModelBase.GeometryDef sourceGeometry)
        {
            ModelBase.GeometryDef geometry = new ModelBase.GeometryDef(sourceGeometry.m_ID);
            foreach (KeyValuePair<string, ModelBase.PolyListDef> polyListEntry in sourceGeometry.m_PolyLists)
            {
                geometry.m_PolyLists[polyListEntry.Key] = DuplicatePolylist(polyListEntry.Value);
            }

            return geometry;
        }

        private void txtModelPreviewScale_TextChanged(object sender, EventArgs e)
        {
            if (Helper.TryParseFloat(txtModelPreviewScale, out m_ModelImportationSettings.m_InGamePreviewScale))
            {
                RefreshBMDScale();
            }
        }

        private void InitTimer()
        {
            m_AnimationTimer = new System.Windows.Forms.Timer();
            m_AnimationTimer.Interval = (int)(1000f / 30f);
            m_AnimationTimer.Tick += new EventHandler(m_AnimationTimer_Tick);
        }

        private void StartTimer()
        {
            m_AnimationFrameNumber = 0;
            m_AnimationTimer.Start();
            m_Running = true;
        }

        private void StopTimer()
        {
            m_AnimationTimer.Stop();
            m_Running = false;
        }

        private void m_AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (m_AnimationFrameNumber < m_AnimationNumFrames - 1)
            {
                IncrementFrame();
            }
            else
            {
                StopTimer();

                if (m_LoopAnimation)
                {
                    StartTimer();
                }
            }
        }

        private void IncrementFrame()
        {
            if (m_AnimationFrameNumber < m_AnimationNumFrames - 1)
                SetFrame(++m_AnimationFrameNumber);
            else
                SetFrame(0);
        }

        private void SetFrame(int frame)
        {
            m_AnimationFrameNumber = frame;

            PrerenderBMDModel();

            txtCurrentFrameNum.Text = "" + m_AnimationFrameNumber;
        }

        private void UpdateCheckBoxes()
        {
            if (m_SelectedMatProp == null)
                return;

            chkDifRed.Checked    = m_SelectedMatProp.difRedAdv;
            chkDifGreen.Checked  = m_SelectedMatProp.difGreenAdv;
            chkDifBlue.Checked   = m_SelectedMatProp.difBlueAdv;
            chkAmbRed.Checked    = m_SelectedMatProp.ambRedAdv;
            chkAmbGreen.Checked  = m_SelectedMatProp.ambGreenAdv;
            chkAmbBlue.Checked   = m_SelectedMatProp.ambBlueAdv;
            chkSpecRed.Checked   = m_SelectedMatProp.specRedAdv;
            chkSpecGreen.Checked = m_SelectedMatProp.specGreenAdv;
            chkSpecBlue.Checked  = m_SelectedMatProp.specBlueAdv;
            chkEmiRed.Checked    = m_SelectedMatProp.emiRedAdv;
            chkEmiGreen.Checked  = m_SelectedMatProp.emiGreenAdv;
            chkEmiBlue.Checked   = m_SelectedMatProp.emiBlueAdv;
            chkAlpha.Checked     = m_SelectedMatProp.alphaAdv;
        }

        private void UpdateTextBoxes()
        {
            if (m_SelectedMatProp == null)
                return;

            txtDifRed.Text    = m_SelectedMatProp.difRedAdv    ? Convert.ToString(m_SelectedMatProp.difRedValues[m_AnimationFrameNumber], 16)    : Convert.ToString(m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Diffuse.R, 16);
            txtDifGreen.Text  = m_SelectedMatProp.difGreenAdv  ? Convert.ToString(m_SelectedMatProp.difGreenValues[m_AnimationFrameNumber], 16)  : Convert.ToString(m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Diffuse.G, 16);
            txtDifBlue.Text   = m_SelectedMatProp.difBlueAdv   ? Convert.ToString(m_SelectedMatProp.difBlueValues[m_AnimationFrameNumber], 16)   : Convert.ToString(m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Diffuse.B, 16);
            txtAmbRed.Text    = m_SelectedMatProp.ambRedAdv    ? Convert.ToString(m_SelectedMatProp.ambRedValues[m_AnimationFrameNumber], 16)    : Convert.ToString(m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Ambient.R, 16);
            txtAmbGreen.Text  = m_SelectedMatProp.ambGreenAdv  ? Convert.ToString(m_SelectedMatProp.ambGreenValues[m_AnimationFrameNumber], 16)  : Convert.ToString(m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Ambient.G, 16);
            txtAmbBlue.Text   = m_SelectedMatProp.ambBlueAdv   ? Convert.ToString(m_SelectedMatProp.ambBlueValues[m_AnimationFrameNumber], 16)   : Convert.ToString(m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Ambient.B, 16);
            txtSpecRed.Text   = m_SelectedMatProp.specRedAdv   ? Convert.ToString(m_SelectedMatProp.specRedValues[m_AnimationFrameNumber], 16)   : Convert.ToString(m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Specular.R, 16);
            txtSpecGreen.Text = m_SelectedMatProp.specGreenAdv ? Convert.ToString(m_SelectedMatProp.specGreenValues[m_AnimationFrameNumber], 16) : Convert.ToString(m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Specular.G, 16);
            txtSpecBlue.Text  = m_SelectedMatProp.specBlueAdv  ? Convert.ToString(m_SelectedMatProp.specBlueValues[m_AnimationFrameNumber], 16)  : Convert.ToString(m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Specular.B, 16);
            txtEmiRed.Text    = m_SelectedMatProp.emiRedAdv    ? Convert.ToString(m_SelectedMatProp.emiRedValues[m_AnimationFrameNumber], 16)    : Convert.ToString(m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Emission.R, 16);
            txtEmiGreen.Text  = m_SelectedMatProp.emiGreenAdv  ? Convert.ToString(m_SelectedMatProp.emiGreenValues[m_AnimationFrameNumber], 16)  : Convert.ToString(m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Emission.G, 16);
            txtEmiBlue.Text   = m_SelectedMatProp.emiBlueAdv   ? Convert.ToString(m_SelectedMatProp.emiBlueValues[m_AnimationFrameNumber], 16)   : Convert.ToString(m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Emission.B, 16);
            txtAlpha.Text     = m_SelectedMatProp.alphaAdv     ? Convert.ToString(m_SelectedMatProp.alphaValues[m_AnimationFrameNumber], 16)     : Convert.ToString(m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Alpha, 16);
        }

        private void UpdateColorButtons()
        {
            btnDif.BackColor = Color.FromArgb(
                m_SelectedMatProp.difRedAdv    ? m_SelectedMatProp.difRedValues[m_AnimationFrameNumber]    : m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Diffuse.R,
                m_SelectedMatProp.difGreenAdv  ? m_SelectedMatProp.difGreenValues[m_AnimationFrameNumber]  : m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Diffuse.G,
                m_SelectedMatProp.difBlueAdv   ? m_SelectedMatProp.difBlueValues[m_AnimationFrameNumber]   : m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Diffuse.B);
            btnAmb.BackColor = Color.FromArgb(
                m_SelectedMatProp.ambRedAdv    ? m_SelectedMatProp.ambRedValues[m_AnimationFrameNumber]    : m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Ambient.R,
                m_SelectedMatProp.ambGreenAdv  ? m_SelectedMatProp.ambGreenValues[m_AnimationFrameNumber]  : m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Ambient.G,
                m_SelectedMatProp.ambBlueAdv   ? m_SelectedMatProp.ambBlueValues[m_AnimationFrameNumber]   : m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Ambient.B);
            btnSpec.BackColor = Color.FromArgb(
                m_SelectedMatProp.specRedAdv   ? m_SelectedMatProp.specRedValues[m_AnimationFrameNumber]   : m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Specular.R,
                m_SelectedMatProp.specGreenAdv ? m_SelectedMatProp.specGreenValues[m_AnimationFrameNumber] : m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Specular.G,
                m_SelectedMatProp.specBlueAdv  ? m_SelectedMatProp.specBlueValues[m_AnimationFrameNumber]  : m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Specular.B);
            btnEmi.BackColor = Color.FromArgb(
                m_SelectedMatProp.emiRedAdv    ? m_SelectedMatProp.emiRedValues[m_AnimationFrameNumber]    : m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Emission.R,
                m_SelectedMatProp.emiGreenAdv  ? m_SelectedMatProp.emiGreenValues[m_AnimationFrameNumber]  : m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Emission.G,
                m_SelectedMatProp.emiBlueAdv   ? m_SelectedMatProp.emiBlueValues[m_AnimationFrameNumber]   : m_ModelBase.m_Materials[lstMaterialProperties.Text].m_Emission.B);
        }

        private void mnitLoad_Click(object sender, EventArgs e)
        {
            m_ROMFileSelect.ReInitialize("Select a BMD file to load", new string[] { ".bmd" });
            DialogResult result = m_ROMFileSelect.ShowDialog();
            if (result != DialogResult.OK)
                return;

            m_Name = m_ROMFileSelect.m_SelectedFile;
            LoadModel();
        }

        private void mnitImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a particle";
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.Cancel) return;
            m_LoadedMatAnim = new BMA(System.IO.File.ReadAllBytes(ofd.FileName));
            m_SelectedMatProp = m_LoadedMatAnim.matProps.FirstOrDefault();

            m_AnimationNumFrames = m_LoadedMatAnim.frames;
            txtNumFrames.Text = $"{m_LoadedMatAnim.frames}";
            txtCurrentFrameNum.Text = "0";

            UpdateCheckBoxes();
            UpdateTextBoxes();
        }

        private void mnitExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog export = new SaveFileDialog();
            export.FileName = $"mat_anim.bma";
            export.Filter = "Binary Material Animation (*.bma) | *.bma";
            if (export.ShowDialog() == DialogResult.Cancel)
                return;
            System.IO.File.WriteAllBytes(export.FileName, m_LoadedMatAnim.GetBMA().m_Data);
        }

        private void btnModelBonesRenameBone_Click(object sender, EventArgs e)
        {

        }
    }
}
