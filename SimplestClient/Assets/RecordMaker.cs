using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class RecordMaker : MonoBehaviour
{


    public InputField m_inputField = null;
    public Button m_CreateButton = null;


    const int BoardSaveDataSignifier = 0;
    const int SaveManagementFileLastUsedIndexIndexSignifier = 1;
    const int SaveManagementFileSignifier = 2;
    static int m_lastIndexUsed;

    static List<string> ReplayofNames;
    SaveManagementFile TempSMF = new SaveManagementFile(1,"jj");
    static LinkedList<SaveManagementFile> m_SaveManagementFiles;

    const string IndexFilePath = "Indices.txt";

    static string LastSelectedName;
    // Start is called before the first frame update
    void Start()
    {
        ReadSaveManagementFile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    
    public void CreateReplayReplay()
    {
        bool isUniqueName = false;
        foreach (SaveManagementFile SMF in m_SaveManagementFiles)
        {
            if (SMF.name == m_inputField.text.ToString())
            {
                SaveReplayRecord(Application.dataPath + Path.DirectorySeparatorChar + SMF.index + ".txt");
                isUniqueName = false;
            }
        }
        if (isUniqueName)
        {
            m_lastIndexUsed++;
            SaveReplayRecord(Application.dataPath + Path.DirectorySeparatorChar + m_lastIndexUsed + ".txt");
            m_SaveManagementFiles.AddLast(new SaveManagementFile(m_lastIndexUsed, m_inputField.text.ToString()));
        }


    }

    static public void WriteSaveManagementFile()
    {
        Debug.Log("SaveManagement Funtion has been called");
        
        StreamWriter sw = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + IndexFilePath);
        sw.WriteLine(SaveManagementFileLastUsedIndexIndexSignifier + "," + m_lastIndexUsed);
        //Debug.Log("1," + lastIndexUsed);

        foreach (SaveManagementFile SMF in m_SaveManagementFiles)
        {
            sw.WriteLine(SaveManagementFileSignifier + "," + SMF.index + "," + SMF.name);
        }
        sw.Close();
    }

    static public void ReadSaveManagementFile()
    {
        m_SaveManagementFiles = new LinkedList<SaveManagementFile>();
        if (File.Exists(Application.dataPath + Path.DirectorySeparatorChar + IndexFilePath))
        {
            StreamReader sr = new StreamReader(Application.dataPath + Path.DirectorySeparatorChar + IndexFilePath);

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                //Debug.Log("line->: "+ line);
                string[] csv = line.Split(',');
                int signifier = int.Parse(csv[0]);
                if (signifier == SaveManagementFileLastUsedIndexIndexSignifier )
                {
                    m_lastIndexUsed = int.Parse(csv[1]);
                    Debug.Log("lastIndexUsed =" + m_lastIndexUsed);
                }
                else if (signifier == SaveManagementFileSignifier)
                {
                    m_SaveManagementFiles.AddLast(new SaveManagementFile(int.Parse(csv[1]), csv[2]));
                }
            }


        }
        /// get the replay name
        ReplayofNames = new List<string>();
        foreach (SaveManagementFile r in m_SaveManagementFiles )
        {
            ReplayofNames.Add(r.name);
        }

    }

    static public void SaveReplayRecord(string fileName)
    {
        Debug.Log("Save ReplayRecord Funtion  has been called");
        StreamWriter sw = new StreamWriter(fileName);
       
            sw.WriteLine( BoardSaveDataSignifier +"," + "0" + "," + "1" + "," + "3" + "," + "4" + "," + "5" + "," + "6" + "," + "7" + "," + "8" );
            
        
        sw.Close();
    }


    public class SaveManagementFile
    {
        public string name;
        public int index;

        public SaveManagementFile(int Index, string Name)
        {
            name = Name;
            index = Index;
        }
        public string GetName
        {
            get { return name; }
        }
        public int GetIndex
        {
            get { return index; }
        }

}
        //public class Record
        //{
        //    private int TotalOFAmonntofMove = 9;
        //    private int [] m_board = new int [9] ;

        //    public SaveManagementFile(int TotalOFAmonntofMove, int [] arrayofInts)
        //    {
        //        m_board = Nam;
        //        index = Index;
        //    }
        //    public string GetName
        //    {
        //        get { return name; }
        //    }
        //    public int GetIndex
        //    {
        //        get { return index; }
        //    }

      //  }

}
