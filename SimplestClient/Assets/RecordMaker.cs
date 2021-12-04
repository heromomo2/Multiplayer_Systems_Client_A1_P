using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class RecordMaker : MonoBehaviour
{

    string m_OurOpponentPlayerName = "Opponent", m_OurPlayerName = "Player";
     
    bool isSelectedRecord = false;
    int maxmove,SelectedMove = 0;
    public List<TicTacToeBoard> MoveBoard = new List<TicTacToeBoard>();
    public Dropdown m_dropdown = null;
    public Text RePlayer_Text,RePlayer_Opponent_Text,RePlayer_Player_Text = null;
    public List<Text> GrideSpace = new List<Text>();
    public NetworkedClient m_MessageReceiverFromServer = null;
    GameObject TicTacToe_Game, Network ;

    LinkedList<TicTacToeBoard> m_allBoards = null;


    //public InputField m_inputField = null;
    //public Button m_CreateButton = null;
    //public Text m_InformationText = null;


    const int BoardSaveDataSignifier = 888;
    const int   PlayerNameSignifier = 67;
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
            m_MessageReceiverFromServer.OnMessageReceivedFromServer += RecordMakerReceived;
        }

        //m_dropdown.options.Clear();
        SetDropDownChanged();
       // LoadDropDownChanged();
        m_dropdown.onValueChanged.AddListener(delegate { LoadDropDownChanged(); });
    
        
    }
    private void OnDestroy()
    {
        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromServer -= RecordMakerReceived;
        }

    }


    void RecordMakerReceived (int signifier, string s, TicTacToeBoard t, MatchData matchData) 
    {
        switch (signifier)
        {
            case ServerToClientSignifiers.ExitTacTacToeComplete:
                ////ResetReplayMaker(1);
                break;
            case ServerToClientSignifiers.RematchOfTicTacToeComplete:
                //ResetReplayMaker(0); 
                break;
            case ServerToClientSignifiers.LoginComplete:
                ResetReplayer();
                SetDropDownChanged();
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    

    public void SetDropDownChanged()
    {
        m_dropdown.options.Clear();
        foreach (string s in ReplayofNames)
        {
            m_dropdown.options.Add(new Dropdown.OptionData() {text = s });
        }
    }

    public void LoadDropDownChanged()
    {
        Debug.Log("LoadDropDownChanged");
        int menuIndex = m_dropdown.GetComponent<Dropdown>().value;
        List<Dropdown.OptionData> menuOptions = m_dropdown.GetComponent<Dropdown>().options;
        string value = menuOptions[menuIndex].text;

        int intdexToload = -1;
        foreach (SaveManagementFile SMF in m_SaveManagementFiles) 
        {
            if(SMF.name == value) 
            {
                intdexToload = SMF.index;
                LastSelectedName = value;
            }
        }

        StreamReader sr = new StreamReader(Application.dataPath + Path.DirectorySeparatorChar + intdexToload + ".txt");
        string line;
        MoveBoard.Clear();
        maxmove = 0;
        SelectedMove = 0;
        while ((line = sr.ReadLine()) != null)
        {
            Debug.Log(line);
            string[] csv = line.Split(',');
            int signifier = int.Parse(csv[0]);
            if (signifier == BoardSaveDataSignifier)
            {
                MoveBoard.Add(new TicTacToeBoard(int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3]), int.Parse(csv[4]), int.Parse(csv[5]), int.Parse(csv[6]), int.Parse(csv[7]), int.Parse(csv[8]), int.Parse(csv[9]), int.Parse(csv[10])));
            }
            else if (signifier == PlayerNameSignifier) 
            {
                m_OurPlayerName = csv[1]; m_OurOpponentPlayerName = csv[2];
            }
        };
        maxmove = MoveBoard.Count;
        isSelectedRecord = true;
        RePlayer_Text.text = "Replayer " + "\n  Move :" + SelectedMove.ToString();
        display(); 
    }
    void display() 
    {
         TicTacToeBoard t = MoveBoard[SelectedMove];
        check(t.topleft,GrideSpace[0]); check(t.topmid, GrideSpace[1]); check(t.topright, GrideSpace[2]);
        check(t.midleft, GrideSpace[3]); check(t.midmid, GrideSpace[4]); check(t.midright, GrideSpace[5]);
        check(t.botleft, GrideSpace[6]); check(t.botmid, GrideSpace[7]); check(t.botright, GrideSpace[8]);

        DisplayWhoturn(t.WhosMove, RePlayer_Player_Text, RePlayer_Opponent_Text);
    }
    void check(int space, Text t)
    {
        if (space == 0) 
        {
            t.text = "";
        }
        else if( space == 1) 
        {
            t.text = "X";
        }
        else if(space == 2)
        {
            t.text = "O";
        }
    }
    void DisplayWhoturn(int Whoturn, Text P, Text O)
    {
        if (Whoturn == 41)
        {
            P.text = "Player:";
            P.color = Color.black;
            O.color = Color.black;
            O.text = "Opponent:";
        }
        else if (Whoturn == 43)
        {
            P.text = m_OurPlayerName + ": Waiting";
            P.color = Color.black;
            O.color = Color.blue;
            O.text = m_OurOpponentPlayerName + ": Turn";
        }
        else if ( Whoturn == 42)
        {
            P.text = m_OurPlayerName + ": Turn";
            P.color = Color.blue;
            O.color = Color.black;
            O.text = m_OurOpponentPlayerName + ": Waiting";
        }
    }

    public void ForwardButtonPressed()
    {
        // pick a list
        if (SelectedMove < maxmove - 1 && isSelectedRecord == true)
        {
  
            SelectedMove= SelectedMove + 1;
            RePlayer_Text.text = "Replayer " + "\n  Move :" + SelectedMove.ToString();
            display();
        }
        //RePlayer_Text.text = "Replayer " + "\n  Move :" + SelectedMove.ToString();
        //display();
        Debug.Log("SelectedMove :-> " + SelectedMove.ToString());
    }
    public void BackwardButtonPressed()
    {
        // pick a list
        if (SelectedMove > 0 && isSelectedRecord == true)
        {
            SelectedMove = SelectedMove - 1;
            RePlayer_Text.text = "Replayer " + "\n  Move :" + SelectedMove.ToString();
            display();
        }
        //RePlayer_Text.text = "Replayer " + "\n  Move :" + SelectedMove.ToString();
        //display();
        Debug.Log("SelectedMove :-> " + SelectedMove.ToString());
    }


    //public void CreateReplayReplay()
    //{
    //    bool isUniqueName = true;
    //    foreach (SaveManagementFile SMF in m_SaveManagementFiles)
    //    {
    //        if (SMF.name == m_inputField.text.ToString())
    //        {
    //            SaveReplayRecord(Application.dataPath + Path.DirectorySeparatorChar + SMF.index + ".txt");
    //            isUniqueName = false;
    //            m_InformationText.text = "Invaild record Name. ";
    //            m_InformationText.color = Color.red;
    //        }
    //    }
    //    if (isUniqueName)
    //    {
    //        m_lastIndexUsed++;
    //        SaveReplayRecord(Application.dataPath + Path.DirectorySeparatorChar + m_lastIndexUsed + ".txt");
    //        m_SaveManagementFiles.AddLast(new SaveManagementFile(m_lastIndexUsed, m_inputField.text.ToString()));
    //        m_InformationText.text = "Success recoded saved. ";
    //        m_InformationText.color = Color.blue;
    //        m_CreateButton.interactable = false;
    //        m_inputField.interactable = false;
    //        RequestToMakeARecordOnTheServer();///
    //        m_inputField.text = "";
    //    }

    //    WriteSaveManagementFile();
    //    ReadSaveManagementFile();
        
    //}

   //private void RequestToMakeARecordOnTheServer() 
   // {
   //     m_MessageReceiverFromServer.SendMessageToHost(ClientToServerSignifiers.CreateARecored+ ","+ m_OurPlayerName + "," + m_inputField.text.ToString());
   // }





    public void ResetReplayMaker(int i)
    {

        //m_CreateButton.interactable = true;
        //m_inputField.interactable = true;
        //m_InformationText.text = "Make a record Bottom or Just continue ";
        //m_InformationText.color = Color.black;
        m_allBoards.Clear();
        Debug.Log(" ResetReplayMaker");
        if ( i == 1) 
        { 
        m_OurPlayerName = "Player";
        m_OurOpponentPlayerName = "Opponent"; 
        }
    }


    public void ResetReplayer()
    {
        foreach (Text t in GrideSpace)
        {
            t.text = "";
        }
        DisplayWhoturn( 41, RePlayer_Player_Text, RePlayer_Opponent_Text);
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

        sw.WriteLine(PlayerNameSignifier+ "," + m_OurPlayerName +"," + m_OurOpponentPlayerName );
        foreach (TicTacToeBoard b in m_allBoards)
        {
            sw.WriteLine(BoardSaveDataSignifier + "," + b.topleft.ToString() + "," + b.topmid.ToString() + "," + b.topright.ToString() + "," + b.midleft.ToString() + "," + b.midmid.ToString() + "," + b.midright.ToString() + "," + b.botleft.ToString() + "," + b.botmid.ToString()+ ","+ b.botright.ToString() +","+ b.WhosMove.ToString());
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
   


   public void Give_TicTacToeBoard ( int topleft, int topmid, int topright, int midleft, int midmid, int midright, int botleft, int botmid, int botright, int wsm)
   {
        m_allBoards.AddLast(new TicTacToeBoard (topleft, topmid,  topright, midleft,  midmid, midright,  botleft,  botmid,  botright, wsm));
   }

    public void GetThePlayerNameRecord( string py, string op)
    {
        m_OurPlayerName = py;
        m_OurOpponentPlayerName = op;
    }


}
