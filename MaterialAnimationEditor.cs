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
        private string m_BmaName;
        private bool m_ModelSourceLoaded;

        private BMD m_LoadedModel;
        private MaterialAnim m_LoadedMatAnim;
        private MaterialAnim.MaterialProperties m_SelectedMatProp;

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

        private bool m_UpdatingWindow = false;

        static MaterialAnimationEditor()
        {
            DUMMY_BMD_DATA = new byte[0x30];
            DUMMY_BMD_NITRO_FILE = new NitroFile();
            DUMMY_BMD_NITRO_FILE.m_ID = 0xFFFF;
            DUMMY_BMD_NITRO_FILE.m_Name = "DUMMY_BMD";
            DUMMY_BMD_NITRO_FILE.m_Data = DUMMY_BMD_DATA;
            DUMMY_BMD = new BMD(DUMMY_BMD_NITRO_FILE);
        }

        public MaterialAnimationEditor(float gameScale = 1f)
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
            UpdateWindow();
        }

        private void MaterialAnimationEditor_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
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

            foreach (var mat in m_ModelBase.m_Materials)
                lstMaterialProperties.Items.Add(mat.Key);
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
                    StartTimer();
            }

            UpdateWindow();
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

        private string ToHexString(uint num)
        {
            return Convert.ToString(num, 16);
        }

        private void SetColourButtonValue(Button button, Color colour)
        {
            string hexColourString = Helper.GetHexColourString(colour);
            button.Text = hexColourString;
            button.BackColor = colour;
            float luma = 0.2126f * colour.R + 0.7152f * colour.G + 0.0722f * colour.B;
            if (luma < 50)
                button.ForeColor = Color.White;
            else
                button.ForeColor = Color.Black;
        }

        private void UpdateWindow()
        {
            m_UpdatingWindow = true;

            chkDifRed.Enabled = chkDifGreen.Enabled = chkDifBlue.Enabled = 
                chkAmbRed.Enabled = chkAmbGreen.Enabled = chkAmbBlue.Enabled = 
                chkSpecRed.Enabled = chkSpecGreen.Enabled = chkSpecBlue.Enabled = 
                chkEmiRed.Enabled = chkEmiGreen.Enabled = chkEmiBlue.Enabled = 
                chkAlpha.Enabled = 
                txtDifRed.Enabled = txtDifGreen.Enabled = txtDifBlue.Enabled =
                txtAmbRed.Enabled = txtAmbGreen.Enabled = txtAmbBlue.Enabled =
                txtSpecRed.Enabled = txtSpecGreen.Enabled = txtSpecBlue.Enabled =
                txtEmiRed.Enabled = txtEmiGreen.Enabled = txtEmiBlue.Enabled =
                txtAlpha.Enabled = m_SelectedMatProp != null && m_LoadedMatAnim != null;
            chkHasAnim.Enabled = lstMaterialProperties.SelectedIndex >= 0;
            btnAddFrame.Enabled = btnRemoveFrame.Enabled = btnPlay.Enabled = btnStop.Enabled =
                btnPreviousFrame.Enabled = btnNextFrame.Enabled = btnFirstFrame.Enabled = btnLastFrame.Enabled =
                m_LoadedMatAnim != null;

            if (m_LoadedMatAnim == null)
            {
                m_UpdatingWindow = false;
                return;
            }

            txtCurrentFrameNum.Text = (m_AnimationFrameNumber + 1) + "";
            txtNumFrames.Text = m_AnimationNumFrames + "";

            chkHasAnim.Checked = m_SelectedMatProp != null;

            if (m_SelectedMatProp == null)
            {
                var mat = GetModelMat();

                label1.Text = "";
                chkDifRed.Checked = chkDifGreen.Checked = chkDifBlue.Checked =
                chkAmbRed.Checked = chkAmbGreen.Checked = chkAmbBlue.Checked =
                chkSpecRed.Checked = chkSpecGreen.Checked = chkSpecBlue.Checked =
                chkEmiRed.Checked = chkEmiGreen.Checked = chkEmiBlue.Checked =
                chkAlpha.Checked = false;

                if (mat != null)
                {
                    ushort dif = Helper.ColorToBGR15(mat.m_Diffuse);
                    ushort amb = Helper.ColorToBGR15(mat.m_Ambient);
                    ushort spec = Helper.ColorToBGR15(mat.m_Specular);
                    ushort emit = Helper.ColorToBGR15(mat.m_Emission);
                    byte alpha = mat.m_Alpha;

                    SetColourButtonValue(btnDif, mat.m_Diffuse);
                    SetColourButtonValue(btnAmb, mat.m_Ambient);
                    SetColourButtonValue(btnSpec, mat.m_Specular);
                    SetColourButtonValue(btnEmi, mat.m_Emission);

                    txtDifRed.Text = ToHexString((byte)((dif >> 0) & 0x1f));
                    txtDifGreen.Text = ToHexString((byte)((dif >> 5) & 0x1f));
                    txtDifBlue.Text = ToHexString((byte)((dif >> 10) & 0x1f));
                    txtAmbRed.Text = ToHexString((byte)((amb >> 0) & 0x1f));
                    txtAmbGreen.Text = ToHexString((byte)((amb >> 5) & 0x1f));
                    txtAmbBlue.Text = ToHexString((byte)((amb >> 10) & 0x1f));
                    txtSpecRed.Text = ToHexString((byte)((spec >> 0) & 0x1f));
                    txtSpecGreen.Text = ToHexString((byte)((spec >> 5) & 0x1f));
                    txtSpecBlue.Text = ToHexString((byte)((spec >> 10) & 0x1f));
                    txtEmiRed.Text = ToHexString((byte)((emit >> 0) & 0x1f));
                    txtEmiGreen.Text = ToHexString((byte)((emit >> 5) & 0x1f));
                    txtEmiBlue.Text = ToHexString((byte)((emit >> 10) & 0x1f));
                    txtAlpha.Text = ToHexString(alpha);

                    label1.Text = "Default material values for " + lstMaterialProperties.SelectedItem.ToString();
                }

                m_UpdatingWindow = false;
                return;
            }

            label1.Text = "Material values for frame " + (m_AnimationFrameNumber + 1) + ":";

            // checkboxes
            chkDifRed.Checked    = m_SelectedMatProp.Adv(PropTypes.difRed);
            chkDifGreen.Checked  = m_SelectedMatProp.Adv(PropTypes.difGreen);
            chkDifBlue.Checked   = m_SelectedMatProp.Adv(PropTypes.difBlue);
            chkAmbRed.Checked    = m_SelectedMatProp.Adv(PropTypes.ambRed);
            chkAmbGreen.Checked  = m_SelectedMatProp.Adv(PropTypes.ambGreen);
            chkAmbBlue.Checked   = m_SelectedMatProp.Adv(PropTypes.ambBlue);
            chkSpecRed.Checked   = m_SelectedMatProp.Adv(PropTypes.specRed);
            chkSpecGreen.Checked = m_SelectedMatProp.Adv(PropTypes.specGreen);
            chkSpecBlue.Checked  = m_SelectedMatProp.Adv(PropTypes.specBlue);
            chkEmiRed.Checked    = m_SelectedMatProp.Adv(PropTypes.emitRed);
            chkEmiGreen.Checked  = m_SelectedMatProp.Adv(PropTypes.emitGreen);
            chkEmiBlue.Checked   = m_SelectedMatProp.Adv(PropTypes.emitBlue);
            chkAlpha.Checked     = m_SelectedMatProp.Adv(PropTypes.alpha);

            // text boxes
            txtDifRed.Text    = ToHexString(m_SelectedMatProp.Value(PropTypes.difRed, m_AnimationFrameNumber));
            txtDifGreen.Text  = ToHexString(m_SelectedMatProp.Value(PropTypes.difGreen, m_AnimationFrameNumber));
            txtDifBlue.Text   = ToHexString(m_SelectedMatProp.Value(PropTypes.difBlue, m_AnimationFrameNumber));
            txtAmbRed.Text    = ToHexString(m_SelectedMatProp.Value(PropTypes.ambRed, m_AnimationFrameNumber));
            txtAmbGreen.Text  = ToHexString(m_SelectedMatProp.Value(PropTypes.ambGreen, m_AnimationFrameNumber));
            txtAmbBlue.Text   = ToHexString(m_SelectedMatProp.Value(PropTypes.ambBlue, m_AnimationFrameNumber));
            txtSpecRed.Text   = ToHexString(m_SelectedMatProp.Value(PropTypes.specRed, m_AnimationFrameNumber));
            txtSpecGreen.Text = ToHexString(m_SelectedMatProp.Value(PropTypes.specGreen, m_AnimationFrameNumber));
            txtSpecBlue.Text  = ToHexString(m_SelectedMatProp.Value(PropTypes.specBlue, m_AnimationFrameNumber));
            txtEmiRed.Text    = ToHexString(m_SelectedMatProp.Value(PropTypes.emitRed, m_AnimationFrameNumber));
            txtEmiGreen.Text  = ToHexString(m_SelectedMatProp.Value(PropTypes.emitGreen, m_AnimationFrameNumber));
            txtEmiBlue.Text   = ToHexString(m_SelectedMatProp.Value(PropTypes.emitBlue, m_AnimationFrameNumber));
            txtAlpha.Text     = ToHexString(m_SelectedMatProp.Value(PropTypes.alpha, m_AnimationFrameNumber));

            // color buttons
            SetColourButtonValue(btnDif, m_SelectedMatProp.GetDif(m_AnimationFrameNumber));
            SetColourButtonValue(btnAmb, m_SelectedMatProp.GetAmb(m_AnimationFrameNumber));
            SetColourButtonValue(btnSpec, m_SelectedMatProp.GetSpec(m_AnimationFrameNumber));
            SetColourButtonValue(btnEmi, m_SelectedMatProp.GetEmit(m_AnimationFrameNumber));

            m_UpdatingWindow = false;
        }

        private void mnitLoad_Click(object sender, EventArgs e)
        {
            m_ROMFileSelect.ReInitialize("Select a BMD file to load", new string[] { ".bmd" });
            DialogResult result = m_ROMFileSelect.ShowDialog();
            if (result != DialogResult.OK)
                return;

            m_LoadedMatAnim = null;
            m_SelectedMatProp = null;
            lstMaterialProperties.SelectedIndex = -1;

            m_Name = m_ROMFileSelect.m_SelectedFile;
            //m_Name = "data/enemy/kuribo/kuribo_model.bmd";
            LoadModel();

            foreach (var x in m_ModelBase.m_Materials) if (!lstMaterialProperties.Items.Contains(x.Key))
                lstMaterialProperties.Items.Add(x.Key);
        }

        private void mnitImport_Click(object sender, EventArgs e)
        {
            m_ROMFileSelect.ReInitialize("Select a BMA file to load", new string[] { ".bma" });
            DialogResult result = m_ROMFileSelect.ShowDialog();
            if (result != DialogResult.OK)
                return;

            m_BmaName = m_ROMFileSelect.m_SelectedFile;
            //m_BmaName = "data/enemy/kuribo/goomba_regurg.bma";
            ReloadBMA();
        }

        private void ReloadBMA()
        {
            lstMaterialProperties.SelectedIndex = -1;

            m_SelectedMatProp = null;
            m_LoadedMatAnim = new MaterialAnim(Program.m_ROM.GetFileFromName(m_BmaName));

            m_AnimationFrameNumber = 0;
            m_AnimationTimer.Stop();

            m_AnimationNumFrames = m_LoadedMatAnim.GetNumFrames();
            
            UpdateWindow();
        }

        private void mnitSave_Click(object sender, EventArgs e)
        {
            m_LoadedMatAnim.SaveFile();
        }

        private void lstMaterialProperties_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMaterialProperties.SelectedIndex < 0 || m_LoadedMatAnim == null)
            {
                m_SelectedMatProp = null;
                return;
            }

            m_SelectedMatProp = m_LoadedMatAnim.GetMatPropFromName(lstMaterialProperties.SelectedItem.ToString());
            UpdateWindow();
        }

        private void btnLastFrame_Click(object sender, EventArgs e)
        {
            m_AnimationFrameNumber = m_LoadedMatAnim.GetNumFrames() - 1;
            UpdateWindow();
        }

        private void btnFirstFrame_Click(object sender, EventArgs e)
        {
            m_AnimationFrameNumber = 0;
            UpdateWindow();
        }

        private void btnNextFrame_Click(object sender, EventArgs e)
        {
            if (m_AnimationFrameNumber == m_LoadedMatAnim.GetNumFrames() - 1)
                return;

            m_AnimationFrameNumber++;
            UpdateWindow();
        }

        private void btnPreviousFrame_Click(object sender, EventArgs e)
        {
            if (m_AnimationFrameNumber == 0)
                return;

            m_AnimationFrameNumber--;
            UpdateWindow();
        }

        private void btnAddFrame_Click(object sender, EventArgs e)
        {
            m_AnimationNumFrames++;
            m_LoadedMatAnim.SetNumFrames((ushort)(m_LoadedMatAnim.GetNumFrames() + 1));
            UpdateWindow();
        }

        private void btnRemoveFrame_Click(object sender, EventArgs e)
        {
            m_AnimationNumFrames--;
            if (m_AnimationFrameNumber == m_LoadedMatAnim.GetNumFrames() - 1)
                m_AnimationFrameNumber--;

            m_LoadedMatAnim.SetNumFrames((ushort)(m_LoadedMatAnim.GetNumFrames() - 1));
            UpdateWindow();
        }

        private ModelBase.MaterialDef GetModelMat()
        {
            if (lstMaterialProperties.SelectedIndex < 0)
                return null;

            return m_ModelBase.m_Materials[lstMaterialProperties.SelectedItem.ToString()];
        }

        private void chkHasAnim_CheckedChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow || m_LoadedMatAnim == null)
                return;

            if (chkHasAnim.Checked)
            {
                string name = lstMaterialProperties.SelectedItem.ToString();
                var matProp = new MaterialAnim.MaterialProperties(name);

                var mat = GetModelMat();

                ushort dif = Helper.ColorToBGR15(mat.m_Diffuse);
                ushort amb = Helper.ColorToBGR15(mat.m_Ambient);
                ushort spec = Helper.ColorToBGR15(mat.m_Specular);
                ushort emit = Helper.ColorToBGR15(mat.m_Emission);
                byte alpha = mat.m_Alpha;

                matProp.InitValue(PropTypes.difRed, (byte)((dif >> 0) & 0x1f));
                matProp.InitValue(PropTypes.difGreen, (byte)((dif >> 5) & 0x1f));
                matProp.InitValue(PropTypes.difBlue, (byte)((dif >> 10) & 0x1f));
                matProp.InitValue(PropTypes.ambRed, (byte)((amb >> 0) & 0x1f));
                matProp.InitValue(PropTypes.ambGreen, (byte)((amb >> 5) & 0x1f));
                matProp.InitValue(PropTypes.ambBlue, (byte)((amb >> 10) & 0x1f));
                matProp.InitValue(PropTypes.specRed, (byte)((spec >> 0) & 0x1f));
                matProp.InitValue(PropTypes.specGreen, (byte)((spec >> 5) & 0x1f));
                matProp.InitValue(PropTypes.specBlue, (byte)((spec >> 10) & 0x1f));
                matProp.InitValue(PropTypes.emitRed, (byte)((emit >> 0) & 0x1f));
                matProp.InitValue(PropTypes.emitGreen, (byte)((emit >> 5) & 0x1f));
                matProp.InitValue(PropTypes.emitBlue, (byte)((emit >> 10) & 0x1f));
                matProp.InitValue(PropTypes.alpha, alpha);

                m_LoadedMatAnim.AddMatProp(matProp);
                m_SelectedMatProp = matProp;
            }
            else
            {
                m_LoadedMatAnim.RemoveMatProp(m_SelectedMatProp);
                m_SelectedMatProp = null;
            }

            UpdateWindow();
        }

        private void chkDifRed_CheckedChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow)
                return;

            if (chkDifRed.Checked)
                m_LoadedMatAnim.EnableProp(m_SelectedMatProp, PropTypes.difRed);
            else
                m_LoadedMatAnim.DisableProp(m_SelectedMatProp, PropTypes.difRed, Convert.ToByte(txtDifRed.Text, 16));
            UpdateWindow();
        }

        private void chkDifGreen_CheckedChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow)
                return;

            if (chkDifGreen.Checked)
                m_LoadedMatAnim.EnableProp(m_SelectedMatProp, PropTypes.difGreen);
            else
                m_LoadedMatAnim.DisableProp(m_SelectedMatProp, PropTypes.difGreen, Convert.ToByte(txtDifGreen.Text, 16));
            UpdateWindow();
        }

        private void chkDifBlue_CheckedChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow)
                return;

            if (chkDifBlue.Checked)
                m_LoadedMatAnim.EnableProp(m_SelectedMatProp, PropTypes.difBlue);
            else
                m_LoadedMatAnim.DisableProp(m_SelectedMatProp, PropTypes.difBlue, Convert.ToByte(txtDifBlue.Text, 16));
            UpdateWindow();
        }

        private void chkAmbRed_CheckedChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow)
                return;

            if (chkAmbRed.Checked)
                m_LoadedMatAnim.EnableProp(m_SelectedMatProp, PropTypes.ambRed);
            else
                m_LoadedMatAnim.DisableProp(m_SelectedMatProp, PropTypes.ambRed, Convert.ToByte(txtAmbRed.Text, 16));
            UpdateWindow();
        }

        private void chkAmbGreen_CheckedChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow)
                return;

            if (chkAmbGreen.Checked)
                m_LoadedMatAnim.EnableProp(m_SelectedMatProp, PropTypes.ambGreen);
            else
                m_LoadedMatAnim.DisableProp(m_SelectedMatProp, PropTypes.ambGreen, Convert.ToByte(txtAmbGreen.Text, 16));
            UpdateWindow();
        }

        private void chkAmbBlue_CheckedChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow)
                return;

            if (chkAmbBlue.Checked)
                m_LoadedMatAnim.EnableProp(m_SelectedMatProp, PropTypes.ambBlue);
            else
                m_LoadedMatAnim.DisableProp(m_SelectedMatProp, PropTypes.ambBlue, Convert.ToByte(txtAmbBlue.Text, 16));
            UpdateWindow();
        }

        private void chkSpecRed_CheckedChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow)
                return;

            if (chkSpecRed.Checked)
                m_LoadedMatAnim.EnableProp(m_SelectedMatProp, PropTypes.specRed);
            else
                m_LoadedMatAnim.DisableProp(m_SelectedMatProp, PropTypes.specRed, Convert.ToByte(txtSpecRed.Text, 16));
            UpdateWindow();
        }

        private void chkSpecGreen_CheckedChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow)
                return;

            if (chkSpecGreen.Checked)
                m_LoadedMatAnim.EnableProp(m_SelectedMatProp, PropTypes.specGreen);
            else
                m_LoadedMatAnim.DisableProp(m_SelectedMatProp, PropTypes.specGreen, Convert.ToByte(txtSpecGreen.Text, 16));
            UpdateWindow();
        }

        private void chkSpecBlue_CheckedChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow)
                return;

            if (chkSpecBlue.Checked)
                m_LoadedMatAnim.EnableProp(m_SelectedMatProp, PropTypes.specBlue);
            else
                m_LoadedMatAnim.DisableProp(m_SelectedMatProp, PropTypes.specBlue, Convert.ToByte(txtSpecBlue.Text, 16));
            UpdateWindow();
        }

        private void chkEmiRed_CheckedChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow)
                return;

            if (chkEmiRed.Checked)
                m_LoadedMatAnim.EnableProp(m_SelectedMatProp, PropTypes.emitRed);
            else
                m_LoadedMatAnim.DisableProp(m_SelectedMatProp, PropTypes.emitRed, Convert.ToByte(txtEmiRed.Text, 16));
            UpdateWindow();
        }

        private void chkEmiGreen_CheckedChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow)
                return;

            if (chkEmiGreen.Checked)
                m_LoadedMatAnim.EnableProp(m_SelectedMatProp, PropTypes.emitGreen);
            else
                m_LoadedMatAnim.DisableProp(m_SelectedMatProp, PropTypes.emitGreen, Convert.ToByte(txtEmiGreen.Text, 16));
            UpdateWindow();
        }

        private void chkEmiBlue_CheckedChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow)
                return;

            if (chkEmiBlue.Checked)
                m_LoadedMatAnim.EnableProp(m_SelectedMatProp, PropTypes.emitBlue);
            else
                m_LoadedMatAnim.DisableProp(m_SelectedMatProp, PropTypes.emitBlue, Convert.ToByte(txtEmiBlue.Text, 16));
            UpdateWindow();
        }

        private void chkAlpha_CheckedChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow)
                return;

            if (chkAlpha.Checked)
                m_LoadedMatAnim.EnableProp(m_SelectedMatProp, PropTypes.alpha);
            else
                m_LoadedMatAnim.DisableProp(m_SelectedMatProp, PropTypes.alpha, Convert.ToByte(txtAlpha.Text, 16));
            UpdateWindow();
        }

        private void txtDifRed_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow || m_Running)
                return;

            try
            {
                byte val = Convert.ToByte(txtDifRed.Text, 16);
                m_SelectedMatProp.SetValue(PropTypes.difRed, m_AnimationFrameNumber, val);
                UpdateWindow();
            }
            catch
            {

            }
        }

        private void txtDifGreen_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow || m_Running)
                return;

            try
            {
                byte val = Convert.ToByte(txtDifGreen.Text, 16);
                m_SelectedMatProp.SetValue(PropTypes.difGreen, m_AnimationFrameNumber, val);
                UpdateWindow();
            }
            catch
            {

            }
        }

        private void txtDifBlue_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow || m_Running)
                return;

            try
            {
                byte val = Convert.ToByte(txtDifBlue.Text, 16);
                m_SelectedMatProp.SetValue(PropTypes.difBlue, m_AnimationFrameNumber, val);
                UpdateWindow();
            }
            catch
            {

            }
        }

        private void txtAmbRed_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow || m_Running)
                return;

            try
            {
                byte val = Convert.ToByte(txtAmbRed.Text, 16);
                m_SelectedMatProp.SetValue(PropTypes.ambRed, m_AnimationFrameNumber, val);
                UpdateWindow();
            }
            catch
            {

            }
        }

        private void txtAmbGreen_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow || m_Running)
                return;

            try
            {
                byte val = Convert.ToByte(txtAmbGreen.Text, 16);
                m_SelectedMatProp.SetValue(PropTypes.ambGreen, m_AnimationFrameNumber, val);
                UpdateWindow();
            }
            catch
            {

            }
        }

        private void txtAmbBlue_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow || m_Running)
                return;

            try
            {
                byte val = Convert.ToByte(txtAmbBlue.Text, 16);
                m_SelectedMatProp.SetValue(PropTypes.ambBlue, m_AnimationFrameNumber, val);
                UpdateWindow();
            }
            catch
            {

            }
        }

        private void txtSpecRed_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow || m_Running)
                return;

            try
            {
                byte val = Convert.ToByte(txtSpecRed.Text, 16);
                m_SelectedMatProp.SetValue(PropTypes.specRed, m_AnimationFrameNumber, val);
                UpdateWindow();
            }
            catch
            {

            }
        }

        private void txtSpecGreen_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow || m_Running)
                return;

            try
            {
                byte val = Convert.ToByte(txtSpecGreen.Text, 16);
                m_SelectedMatProp.SetValue(PropTypes.specGreen, m_AnimationFrameNumber, val);
                UpdateWindow();
            }
            catch
            {

            }
        }

        private void txtSpecBlue_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow || m_Running)
                return;

            try
            {
                byte val = Convert.ToByte(txtSpecBlue.Text, 16);
                m_SelectedMatProp.SetValue(PropTypes.specBlue, m_AnimationFrameNumber, val);
                UpdateWindow();
            }
            catch
            {

            }
        }

        private void txtEmiRed_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow || m_Running)
                return;

            try
            {
                byte val = Convert.ToByte(txtEmiRed.Text, 16);
                m_SelectedMatProp.SetValue(PropTypes.emitRed, m_AnimationFrameNumber, val);
                UpdateWindow();
            }
            catch
            {

            }
        }

        private void txtEmiGreen_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow || m_Running)
                return;

            try
            {
                byte val = Convert.ToByte(txtEmiGreen.Text, 16);
                m_SelectedMatProp.SetValue(PropTypes.emitGreen, m_AnimationFrameNumber, val);
                UpdateWindow();
            }
            catch
            {

            }
        }

        private void txtEmiBlue_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow || m_Running)
                return;

            try
            {
                byte val = Convert.ToByte(txtEmiBlue.Text, 16);
                m_SelectedMatProp.SetValue(PropTypes.emitBlue, m_AnimationFrameNumber, val);
                UpdateWindow();
            }
            catch
            {

            }
        }

        private void txtAlpha_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingWindow || m_Running)
                return;

            try
            {
                byte val = Convert.ToByte(txtAlpha.Text, 16);
                m_SelectedMatProp.SetValue(PropTypes.alpha, m_AnimationFrameNumber, val);
                UpdateWindow();
            }
            catch
            {

            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            StartTimer();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopTimer();
        }
    }
}
