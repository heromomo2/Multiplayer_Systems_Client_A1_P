using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class RecordMaker : MonoBehaviour
{
    
    public Text RePlayer_Text = null;
    public List<Text> GrideSpace = new List<Text>();
    public NetworkedClient m_MessageReceiverFromServer = null;
    GameObject TicTacToe_Game, Network ;

    LinkedList<TicTacToeBoard> m_allBoards = null;


    public InputField m_inputField = null;
    public Button m_CreateButton = null;
    public Text m_InformationText = null;


    const int BoardSaveDataSignifier = 888;
    const int SaveManagementFileLastUsedIndexIndexSignifier = 1;
    const int SaveManagementFileSignifier = 2;
    static int m_lastIndexUsed;

     List<string> ReplayofNames;
    SaveManagementFile TempSMF = new SaveManagementFile(1, "jj");
     LinkedList<SaveManagementFile> m_SaveManagementFiles;

    const string IndexFilePath = "Indices.txt";

    static string LastSelectedName;
    // Start is called before the first frame update
    void Start()

    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "Game_Ui")
                TicTacToe_Game = go;
            else if (go.name == "Network")
                Network = go;
        }
        ReadSaveManagementFile();
        m_allBoards = new LinkedList<TicTacToeBoard>();

        m_MessageReceiverFromServer = Network.GetComponent<NetworkedClient>();

        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromSever += RecordMakerReceived;
        }
    }
    private void OnDestroy()
    {
        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromSever -= RecordMakerReceived;
        }

    }


    void RecordMakerReceived (int sigifier, string s) 
    {
        switch (sigifier)
        {
            case ServerToClientSignifiers.ExitTacTacToeComplete:
                ResetReplayMaker();
                break;
            case ServerToClientSignifiers.ReMatchOfTicTacToeComplete:
                ResetReplayMaker(); 
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

     public List<string> GetListOfReplayerByNames()
    {



        return ReplayofNames;   
    }

     public void LoadReplayDropDownChanged(string selectedName)
    {
       
    }

    public void ForwardButtonPressed()
    {
        // pick a list
    }
    public void BackwardButtonPressed()
    {
        // pick a list
    }


    public void CreateReplayReplay()
    {
        bool isUniqueName = true;
        foreach (SaveManagementFile SMF in m_SaveManagementFiles)
        {
            if (SMF.name == m_inputField.text.ToString())
            {
                SaveReplayRecord(Application.dataPath + Path.DirectorySeparatorChar + SMF.index + ".txt");
                isUniqueName = false;
                m_InformationText.text = "Invaild record Name. ";
                m_InformationText.color = Color.red;
            }
        }
        if (isUniqueName)
        {
            m_lastIndexUsed++;
            SaveReplayRecord(Application.dataPath + Path.DirectorySeparatorChar + m_lastIndexUsed + ".txt");
            m_SaveManagementFiles.AddLast(new SaveManagementFile(m_lastIndexUsed, m_inputField.text.ToString()));
            m_InformationText.text = "Success recoded saved. ";
            m_InformationText.color = Color.blue;
            m_CreateButton.interactable = false;
            m_inputField.interactable = false;
            m_inputField.text = "";
        }


        WriteSaveManagementFile();
        ReadSaveManagementFile();

      
    }


    public void ResetReplayMaker() 
    {
        m_CreateButton.interactable = true;
        m_inputField.interactable = true;
        m_InformationText.text = "Make a record Bottom or Just continue ";
        m_InformationText.color = Color.black;
        Debug.Log(" ResetReplayMaker");
    }




    public void WriteSaveManagementFile()
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

     public void ReadSaveManagementFile()
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
                if (signifier == SaveManagementFileLastUsedIndexIndexSignifier)
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
        foreach (SaveManagementFile r in m_SaveManagementFiles)
        {
            ReplayofNames.Add(r.name);
        }

    }

     public void SaveReplayRecord(string fileName)
    {
        Debug.Log("Save ReplayRecord Funtion  has been called");
        StreamWriter sw = new StreamWriter(fileName);

        foreach (TicTacToeBoard b in m_allBoards)
        {
            sw.WriteLine(BoardSaveDataSignifier + "," + b.topleft.ToString() + "," + b.topmid.ToString() + "," + b.topright.ToString() + "," + b.midleft.ToString() + "," + b.midmid.ToString() + "," + b.midright.ToString() + "," + b.botleft.ToString() + "," + b.botmid.ToString()+ ","+ b.botright);
        }

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
   


    public void Give_TicTacToeBoard ( int topleft, int topmid, int topright, int midleft, int midmid, int midright, int botleft, int botmid, int botright)
   {
        m_allBoards.AddLast(new TicTacToeBoard (topleft, topmid,  topright, midleft,  midmid, midright,  botleft,  botmid,  botright));
   }


    public class TicTacToeBoard
    {
        public int topleft, topmid, topright, midleft, midmid, midright, botleft, botmid, botright;


        public TicTacToeBoard(int tl, int tm, int tr, int ml, int mm, int mr, int bl, int bm, int br)
        {
            topleft = tl;
            topmid = tm;
            topright = tr;
            midleft = ml;
            midmid = mm;
            midright = mr;
            botleft = bl;
            botmid = bm;
            botright = br;
        }

    }

}
