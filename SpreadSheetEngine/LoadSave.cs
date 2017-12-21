using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace Cpts321
{
    class LoadSave
    {
        public LoadSave()
        {

        }

        public void Load()
        {
            /*
            StreamReader InFile = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"; // only allows .txt file type
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((InFile = File.OpenText(openFileDialog1.FileName)) != null) // opens dialog box
                    {
                        LoadFile(InFile); // my Load Function
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }

            InFile.Close();
            //----------------------------------

            StreamReader InFile = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"; // only allows .txt file type
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;



            using (XmlReader reader = XmlReader.Create(@"Save.xml"))
            {

                // Move to the hire-date element.
                reader.MoveToContent();
                reader.ReadToDescendant("hire-date");

                // Return the hire-date as a DateTime object.
                DateTime hireDate = reader.ReadElementContentAsDateTime();
                Console.WriteLine("Six Month Review Date: {0}", hireDate.AddMonths(6));
            }
            

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<item><name>wrench</name></item>");
        }
        public void Save()
        {

        }
        */
        }
    }
}
