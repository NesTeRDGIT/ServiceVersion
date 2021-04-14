using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Threading;
using ServiceLoaderMedpomData;
using System.Xml.XPath;

namespace ClientService.SchemaEditor
{
    public partial class XMLschemaEditor : Form
    {
        public XMLschemaEditor()
        {
            InitializeComponent();

            XMLshemaMy = new XMLSchemaFile();
            comboBoxType.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
        }

        TypeS currenttype;
        TypeSComplex currComplex = new TypeSComplex();
        TypeSString currString = new TypeSString();
        TypeSDigit currDigit = new TypeSDigit();
        TypeSDate currDate = new TypeSDate();
        TypeSTime currTime = new TypeSTime();
        XMLSchemaFile XMLshemaMy;
        private void XMLshema_Load(object sender, EventArgs e)
        {
            currString.ZnakMest = 2;
            currenttype = currString;
            label8.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text == "")
            {
                errorProvider1.SetError(textBoxName, "Имя не может быть пустым");
                return;
            }
            if (treeView1.SelectedNode != null)
            {
                int[] index = GetPath(treeView1.SelectedNode);
                XMLshemaMy.Insert(index, textBoxName.Text, (TypeElement)comboBoxType.SelectedIndex, currenttype);
                index[index.Length-1]++;
                SchemaElement el =  XMLshemaMy[index];
                    TreeNode Node = new TreeNode();
                    Node.Text = el.name + " {" + el.Type.ToString() + "} (" + el.format.toSTR() + ")";
                    //Node.Tag = XMLprogect.XMLSchemaFile.ConvertPathtoStr(index);
                    //Node.Name = (string)Node.Tag;
                    if (el.format is TypeSDigit)
                        Node.ImageIndex = 1;
                    if (el.format is TypeSString)
                        Node.ImageIndex = 2;
                    if (el.format is TypeSDate)
                        Node.ImageIndex = 0;
                    if (el.format is TypeSComplex)
                        Node.ImageIndex = 3;
                    if (treeView1.SelectedNode.Parent!=null)
                        treeView1.SelectedNode.Parent.Nodes.Insert(treeView1.SelectedNode.Index+1, Node);
                    else
                        treeView1.Nodes.Insert(treeView1.SelectedNode.Index+1, Node);

            }
            else
            {
                XMLshemaMy.Add(new int[0], textBoxName.Text, (TypeElement)comboBoxType.SelectedIndex, currenttype);
                
                SchemaElement el = XMLshemaMy[new int[]{0}];
                TreeNode Node = new TreeNode();
                Node.Text = el.name + " {" + el.Type.ToString() + "} (" + el.format.toSTR() + ")";


                if (el.format is TypeSDigit)
                    Node.ImageIndex = 1;
                if (el.format is TypeSString)
                    Node.ImageIndex = 2;
                if (el.format is TypeSDate)
                    Node.ImageIndex = 0;
                if (el.format is TypeSComplex)
                    Node.ImageIndex = 3;

                treeView1.Nodes.Add(Node);
            }
            
        }



        void SetElementCurrType()
        {
            if (currenttype is TypeSComplex)
            {
              comboBox1.SelectedIndex = 3;
            }
            if (currenttype is TypeSString)
            {
                comboBox1.SelectedIndex = 0;
                numericUpDownLength.Value = (currenttype as TypeSString).ZnakMest;
                listBoxEnumString.Items.Clear();
                currString.Enum = (currenttype as TypeSString).Enum;
                foreach(string str in (currenttype as TypeSString).Enum)
                {
                    listBoxEnumString.Items.Add(str);
                }
            }
            if (currenttype is TypeSDate)
            {
                comboBox1.SelectedIndex = 2;


            }
            if (currenttype is TypeSDigit)
            {
                comboBox1.SelectedIndex = 1;
                numericUpDownTotalDigit.Value = (currenttype as TypeSDigit).ZnakMest;                
                numericUpDownDigitZap.Value = (currenttype as TypeSDigit).ZnakMestPosDot;
                listBoxDigitEnum.Items.Clear();
                currDigit.Enum = (currenttype as TypeSDigit).Enum;
                foreach (int str in (currenttype as TypeSDigit).Enum)
                {
                    listBoxDigitEnum.Items.Add(str);
                }
            }

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {


                if (treeView1.SelectedNode != null)
                {
                    int[] index = GetPath(treeView1.SelectedNode);
                    SchemaElement item = XMLshemaMy[index];
                    textBoxName.Text = item.name;
                    textBoxFormat.Text = item.format.toSTRRUS();
                    if (item.format is TypeSDigit)
                    {
                        checkBoxIndexNum.Checked = item.Unique;
                        checkBoxIndexGlobalNum.Checked = item.UniqueGlobal;
                    }
                    if (item.format is TypeSString)
                    {
                        checkBoxIndexSTR.Checked = item.Unique;
                        checkBoxIndexGlobalStr.Checked = item.UniqueGlobal;
                    }

                    currenttype = item.format;
                    SetElementCurrType();
                    comboBoxType.SelectedIndex = (int)item.Type;
                    if (item.format is TypeSComplex)
                        buttonCreateBranch.Enabled = true;
                    else
                        buttonCreateBranch.Enabled = false;
                    buttonChange.Enabled = true;
                    treeView1.SelectedImageIndex = treeView1.SelectedNode.ImageIndex;

                }
                else
                {
                    buttonChange.Enabled = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            RefreshTreeView();
        }

        private void RefreshTreeView()
        {            
            if (treeView1.SelectedNode != null)
            {
                //string selected = treeView1.SelectedNode.Name;
                treeView1.Nodes.Clear();
                FindP.Clear();
                RefreshTree(treeView1.Nodes, new int[1]);
                //TreeNode[] tr = treeView1.Nodes.Find(selected, true);
                //if(tr.Count()!=0)
                  //  treeView1.SelectedNode = tr[0];
            }
            else
            {
                treeView1.Nodes.Clear();
                RefreshTree( treeView1.Nodes, new int[1]);
            }
        }

        private void RefreshTree(TreeNodeCollection Nodes, int[] index)
        {
            int[] rootindex = new int[index.Length - 1];
            for (int i = 0; i < rootindex.Length; i++)
            {
                rootindex[i] = index[i];
            }

            for (int i = 0; i < XMLshemaMy.Count(rootindex); i++)
            {
                TreeNode Node = new TreeNode();
                index[index.Length - 1] = (int) i;
                Node.Text = XMLshemaMy[index].name + " {" + XMLshemaMy[index].Type.ToString() + "} (" +
                            XMLshemaMy[index].format.toSTR() + ")";
                //Node.Tag = XMLprogect.XMLSchemaFile.ConvertPathtoStr(index);
                //Node.Name = (string)Node.Tag;
                if (XMLshemaMy[index].format is TypeSDigit)
                    Node.ImageIndex = 1;
                if (XMLshemaMy[index].format is TypeSString)
                    Node.ImageIndex = 2;
                if (XMLshemaMy[index].format is TypeSDate)
                    Node.ImageIndex = 0;
                if (XMLshemaMy[index].format is TypeSComplex)
                    Node.ImageIndex = 3;
                Nodes.Add(Node);
                if (XMLshemaMy[index].Elements != null && XMLshemaMy[index].Elements.Count > 0)
                {
                    int[] currentindex = new int[index.Length + 1];
                    index.CopyTo(currentindex, 0);
                    RefreshTree(Node.Nodes, currentindex);
                }
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text == "")
            {
                errorProvider1.SetError(textBoxName, "Имя не может быть пустым");
                return;
            }
            if (treeView1.SelectedNode != null)
            {
                int[] indextmp = GetPath(treeView1.SelectedNode);


                SchemaElement el = XMLshemaMy.Add(indextmp, textBoxName.Text, (TypeElement)comboBoxType.SelectedIndex, currenttype);



                TreeNode Node = new TreeNode();
                Node.Text = el.name + " {" + el.Type.ToString() + "} (" + el.format.toSTR() + ")";

                if (el.format is TypeSDigit)
                    Node.ImageIndex = 1;
                if (el.format is TypeSString)
                    Node.ImageIndex = 2;
                if (el.format is TypeSDate)
                    Node.ImageIndex = 0;
                if (el.format is TypeSComplex)
                    Node.ImageIndex = 3;


                if ((el.format is TypeSDigit))
                {
                    el.Unique = checkBoxIndexNum.Checked;
                    el.UniqueGlobal = checkBoxIndexGlobalNum.Checked;
                }
                if ((el.format is TypeSString))
                {
                    el.Unique = checkBoxIndexSTR.Checked;
                    el.UniqueGlobal = checkBoxIndexGlobalStr.Checked;
                }
                    treeView1.SelectedNode.Nodes.Add(Node);


            }
           
        }

        private void XMLshema_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBoxName.Text == "")
                {
                    errorProvider1.SetError(textBoxName, "Имя не может быть пустым");
                    return;
                }
                SchemaElement se = XMLshemaMy[GetPath(treeView1.SelectedNode)];
                se.name = textBoxName.Text;
                se.Type = (TypeElement)comboBoxType.SelectedIndex;
                TypeS newT = null;
                if (currenttype is TypeSComplex)
                {
                    newT = new TypeSComplex();

                }
                if (currenttype is TypeSDate)
                    newT = new TypeSDate();
                if (currenttype is TypeSString)
                {
                    newT = new TypeSString(currString.ZnakMest, currString.Enum);

                }
                if (currenttype is TypeSDigit)
                {
                    newT = new TypeSDigit(currDigit.ZnakMest, currDigit.ZnakMestPosDot, currDigit.Enum);
                }
                if (currenttype is TypeSTime)
                {
                    newT = new TypeSTime();
                }

                if (!(newT is TypeSComplex))
                    se.Elements = null;


                se.format = newT;
                if ((se.format is TypeSDigit))
                {
                    se.Unique = checkBoxIndexNum.Checked;
                    se.UniqueGlobal = checkBoxIndexGlobalNum.Checked;
                }
                if ((se.format is TypeSString))
                {
                    se.Unique = checkBoxIndexSTR.Checked;
                    se.UniqueGlobal = checkBoxIndexGlobalStr.Checked;
                }
                XMLshemaMy[GetPath(treeView1.SelectedNode)] = se;

                // RefreshTreeView();            
                TreeNode Node = treeView1.SelectedNode;
                RefreshNodeTreeView(Node);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }


        private void RefreshNodeTreeView(TreeNode Node)
        {
                    int[] index = GetPath(Node);
                    Node.Text = XMLshemaMy[index].name + " {" + XMLshemaMy[index].Type.ToString() + "} (" + XMLshemaMy[index].format.toSTR() + ")";
                    SchemaElement se = XMLshemaMy[index];
                    if (se.format is TypeSDigit)
                        Node.ImageIndex = 1;
                    if (se.format is TypeSString)
                        Node.ImageIndex = 2;
                    if (se.format is TypeSDate)
                        Node.ImageIndex = 0;
                    if (se.format is TypeSComplex)
                        Node.ImageIndex = 3;
                    treeView1.SelectedImageIndex = Node.ImageIndex;

        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                var node = treeView1.SelectedNode;
                var t = GetPath(node);
                node.Remove();
                XMLshemaMy.RemoveAt(t);
                //RefreshTreeView();
            }
        }

        private int[] GetPath(TreeNode node)
        {
            int[] index = new int[node.Level+1];
            for (int i = node.Level; i >=0; i--)
            {
                index[i] = node.Index;
                node = node.Parent;
            }
            return index;
        }

        private void buttonDeleteAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($@"Не сохраненные данные будут потеряны!{Environment.NewLine}Удалить все?", @"Подтверждение", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                XMLshemaMy.Clear();
                RefreshTreeView();
            }
        }

        private void buttonUpElement_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null) return;
            if (XMLshemaMy.ElementUp(GetPath(treeView1.SelectedNode)))
            {
                TreeNode Prevnode = treeView1.SelectedNode.PrevNode;
                TreeNode currnode = treeView1.SelectedNode;
                string tmp = currnode.Text;
                currnode.Text = Prevnode.Text;
                Prevnode.Text = tmp;
                treeView1.SelectedNode = Prevnode;

            }
        }

        private void buttonDownElement_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null) return;
            if (XMLshemaMy.ElementDown(GetPath(treeView1.SelectedNode)))
            {
                TreeNode NextNode = treeView1.SelectedNode.NextNode;
                TreeNode currnode = treeView1.SelectedNode;
                string tmp = currnode.Text;
                currnode.Text = NextNode.Text;
                NextNode.Text = tmp;
                treeView1.SelectedNode = NextNode;
            }
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            saveFileDialog2.FileName = Path.GetFileNameWithoutExtension(label8.Text);
            if (saveFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                if (XMLshemaMy.Compile(saveFileDialog2.FileName))
                {
                    MessageBox.Show(@"Создание схемы успешно!");
                }

        }

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            treeView1.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Effect == DragDropEffects.Move)
            {
                TreeNode hoveringNode = GetHoveringNode(e.X, e.Y);
                if (hoveringNode != null)
                {
                    TreeNode draggingNode = e.Data.GetData(typeof(TreeNode)) as TreeNode;
                    if (draggingNode != null)
                    {
                        draggingNode.Remove();
                        SchemaElement el = XMLshemaMy[GetPath(draggingNode)];
                        XMLshemaMy.Insert(GetPath(hoveringNode), el.name, el.Type, el.format);
                        XMLshemaMy.RemoveAt(GetPath(draggingNode));
                        if (hoveringNode.Parent != null)
                            hoveringNode.Parent.Nodes.Insert(hoveringNode.Index+1, draggingNode);
                        else
                            treeView1.Nodes.Insert(hoveringNode.Index + 1, draggingNode);
                        RefreshTreeView();

                        //hoveringNode.Nodes.Add(draggingNode);

                    }
                }
            }
        }

        private TreeNode GetHoveringNode(int screen_x, int screen_y)
        {
            Point pt = treeView1.PointToClient(new Point(screen_x, screen_y));
            TreeViewHitTestInfo hitInfo = treeView1.HitTest(pt);
            return hitInfo.Node;
        }

        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            TreeNode hoveringNode = GetHoveringNode(e.X, e.Y);
            TreeNode draggingNode = e.Data.GetData(typeof(TreeNode)) as TreeNode;
            if (hoveringNode != null && hoveringNode != draggingNode && draggingNode != hoveringNode.Parent)
            {
                e.Effect = DragDropEffects.Move;
                hoveringNode.TreeView.SelectedNode = hoveringNode;
                hoveringNode.Expand();
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {

                case 0:
                    tabControl1.SelectedTab = tabControl1.TabPages[1];
                    currenttype = currString;
                    break;
                case 1: tabControl1.SelectedTab = tabControl1.TabPages[0];
                    currenttype = currDigit;
                    break;
                case 2:                    
                    tabControl1.SelectedTab = tabControl1.TabPages[3];
                    currenttype = currDate;
                    break;
                case 3:                    
                    tabControl1.SelectedTab = tabControl1.TabPages[3];
                    currenttype = currComplex;
                    break;
                case 4:
                    tabControl1.SelectedTab = tabControl1.TabPages[3];
                    currenttype = currTime;
                    break;
                default:
                    tabControl1.SelectedTab = tabControl1.TabPages[3];
                    break;
            }
            textBoxFormat.Text = currenttype.toSTRRUS();
        }

     
       private void buttonLoad_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog()== System.Windows.Forms.DialogResult.OK)
            {
                XMLshemaMy.LoadFromFile(openFileDialog1.FileName);
                label8.Text = openFileDialog1.FileName;
                RefreshTreeView();
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (label8.Text == "")
            {
                saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(label8.Text);
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    XMLshemaMy.SaveToFile(saveFileDialog1.FileName);
                    label8.Text = saveFileDialog1.FileName;
                }
            }
            else
            {
                XMLshemaMy.SaveToFile(label8.Text);                
            }
        }

          

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (textBoxEnumItem.Text != "")
            {
                currDigit.Enum.Add(Convert.ToInt32(textBoxEnumItem.Text));
                listBoxDigitEnum.Items.Add(textBoxEnumItem.Text);
                textBoxFormat.Text = currDigit.toSTRRUS();
            }

            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBoxDigitEnum.SelectedIndex != -1)
            {
                currDigit.Enum.RemoveAt(listBoxDigitEnum.SelectedIndex);
                listBoxDigitEnum.Items.RemoveAt(listBoxDigitEnum.SelectedIndex);
                textBoxFormat.Text = currDigit.toSTRRUS();
            }

        }

        private void numericUpDownTotalDigit_ValueChanged(object sender, EventArgs e)
        {
            currDigit.ZnakMest = (int)numericUpDownTotalDigit.Value;
            textBoxFormat.Text = currDigit.toSTRRUS();
        }

        private void numericUpDownDigitZap_ValueChanged(object sender, EventArgs e)
        {
            currDigit.ZnakMestPosDot = (int)numericUpDownDigitZap.Value;
            textBoxFormat.Text = currDigit.toSTRRUS();
        }

        private void numericUpDownLength_ValueChanged(object sender, EventArgs e)
        {
            currString.ZnakMest = (int)numericUpDownLength.Value;
            textBoxFormat.Text = currString.toSTRRUS();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (textBoxEnumString.Text != "")
            {
                currString.Enum.Add(textBoxEnumString.Text);
                listBoxEnumString.Items.Add(textBoxEnumString.Text);
                textBoxFormat.Text = currString.toSTRRUS();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBoxEnumString.SelectedIndex != -1)
            {
                currString.Enum.RemoveAt(listBoxEnumString.SelectedIndex);
                listBoxEnumString.Items.RemoveAt(listBoxEnumString.SelectedIndex);
                textBoxFormat.Text = currString.toSTRRUS();
            }
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(textBoxName, "");
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                XMLshemaMy.SaveToFile(saveFileDialog1.FileName);
                label8.Text = saveFileDialog1.FileName;
            }
        }

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label8.Text = "";
        }
        List<SchemaElement> CopyItem = new List<SchemaElement>();
        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                CopyItem.Clear();
                int[] index = GetPath(treeView1.SelectedNode);
                CopyItem.Add(XMLshemaMy[index]);
            }
        }

        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                int[] index = GetPath(treeView1.SelectedNode);
                for(int i = CopyItem.Count-1;i>=0;i--)              
                {
                    XMLshemaMy.Insert(index, CopyItem[i]);
                    RefreshTreeView();
                }
               
            }
        }

        private void копироватьОтмеченныеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var l = GetChecked(treeView1.Nodes);
            if(l.Count!=0)
            {
                CopyItem.Clear();
                foreach (var n in l)
                {
                    int[] index = GetPath(n);
                    CopyItem.Add(XMLshemaMy[index]);
                 
                }
            }
           
        }

        public List<TreeNode> GetChecked(TreeNodeCollection tr)
        {
            List<TreeNode> res = new List<TreeNode>();
            foreach(TreeNode t in tr)
            {
                if(t.Checked)
                {
                    res.Add(t);
                }
                else
                {
                    res.AddRange(GetChecked(t.Nodes));
                }
            }
            return res;
        }

        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public class FindPredicat
        {
            public string FindText { get; set; }
            public List<TreeNode> FindNODE { get; set; } = new List<TreeNode>();
            private int CurrItem { get; set; }

            public void Clear()
            {
                FindText = null;
                FindNODE.Clear();
                CurrItem = -1;
            }

            public TreeNode GetNEXT()
            {
                CurrItem++;
                if (CurrItem<FindNODE.Count) return FindNODE[CurrItem];
                return null;
            }
            
        }


        FindPredicat FindP = new FindPredicat();

        private void button3_Click_1(object sender, EventArgs e)
        {
            try
            {
                string findstr = textBoxFIND_ITEM.Text.ToUpper();
                if (!string.IsNullOrEmpty(findstr))
                {
                    if (FindP.FindText != findstr)
                    {
                        FindP.Clear();
                        FindP.FindText = findstr;
                        FindP.FindNODE = FindNODE(FindP.FindText, treeView1.Nodes);
                    }

                    var nod = FindP.GetNEXT();
                    if (nod != null)
                    {
                        treeView1.SelectedNode = nod;
                    }
                    else
                    {
                        MessageBox.Show(@"Поиск достиг конца списка");
                        FindP.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                FindP.Clear();
            }
          
        }

        List<TreeNode> FindNODE(string FIND,TreeNodeCollection coll)
        {
            var res = new List<TreeNode>();
            foreach (TreeNode NODE in coll)
            {
                if (NODE.Text.ToUpper().Contains(FIND))
                {
                    res.Add(NODE);
                }
                res.AddRange(FindNODE(FIND, NODE.Nodes));
            }
            return res;
        }

        private void компиляцияToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
