
/*
This RPG data streaming assignment was created by Fernando Restituto.
Pixel RPG characters created by Sean Browning.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#region Assignment Instructions

/*  Hello!  Welcome to your first lab :)

Wax on, wax off.

    The development of saving and loading systems shares much in common with that of networked gameplay development.  
    Both involve developing around data which is packaged and passed into (or gotten from) a stream.  
    Thus, prior to attacking the problems of development for networked games, you will strengthen your abilities to develop solutions using the easier to work with HD saving/loading frameworks.

    Try to understand not just the framework tools, but also, 
    seek to familiarize yourself with how we are able to break data down, pass it into a stream and then rebuild it from another stream.


Lab Part 1

    Begin by exploring the UI elements that you are presented with upon hitting play.
    You can roll a new party, view party stats and hit a save and load button, both of which do nothing.
    You are challenged to create the functions that will save and load the party data which is being displayed on screen for you.

    Below, a SavePartyButtonPressed and a LoadPartyButtonPressed function are provided for you.
    Both are being called by the internal systems when the respective button is hit.
    You must code the save/load functionality.
    Access to Party Character data is provided via demo usage in the save and load functions.

    The PartyCharacter class members are defined as follows.  */

public partial class PartyCharacter
{
    public int classID;

    public int health;
    public int mana;

    public int strength;
    public int agility;
    public int wisdom;

    public LinkedList<int> equipment;

}

/*
    Access to the on screen party data can be achieved via …..

    Once you have loaded party data from the HD, you can have it loaded on screen via …...

    These are the stream reader/writer that I want you to use.
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamwriter
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamreader

    Alright, that’s all you need to get started on the first part of this assignment, here are your functions, good luck and journey well!
*/
#endregion

enum GameDataId
{
    CharacterDataId,
    EquipmentDataId
}

#region Assignment Part 1
static public class AssignmentPart1
{
    static public void SavePartyButtonPressed()
    {
        StreamWriter sw = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + "SaveFile.txt");
        foreach (PartyCharacter pc in GameContent.partyCharacters)
        {
            Debug.Log("PC class id == " + pc.classID);

            sw.WriteLine((int)GameDataId.CharacterDataId + "," + pc.classID + "," + pc.health + "," + pc.mana + "," + pc.strength + "," + pc.agility + "," + pc.wisdom);

            foreach (int item in pc.equipment)
            {
                sw.WriteLine((int)GameDataId.EquipmentDataId + "," + item);
            }
        }
        sw.Close();
    }

    static public void LoadPartyButtonPressed()
    {
        GameContent.partyCharacters.Clear();

        StreamReader sr = new StreamReader(Application.dataPath + Path.DirectorySeparatorChar + "SaveFile.txt");

        string line;
        while ((line = sr.ReadLine()) != null)
        {
            Debug.Log(line);

            string[] csv = line.Split(',');

            int game_data_id = int.Parse(csv[0]);

            if (game_data_id == (int)GameDataId.CharacterDataId)
            {
                GameContent.partyCharacters.AddLast(new PartyCharacter(int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3]), int.Parse(csv[4]), int.Parse(csv[5]), int.Parse(csv[6])));
            }
            else if (game_data_id == (int)GameDataId.EquipmentDataId)
            {
                GameContent.partyCharacters.Last.Value.equipment.AddLast(int.Parse(csv[1]));
            }
        }
        sr.Close();

        GameContent.RefreshUI();
    }
}
#endregion


#region Assignment Part 2

//  Before Proceeding!
//  To inform the internal systems that you are proceeding onto the second part of this assignment,
//  change the below value of AssignmentConfiguration.PartOfAssignmentInDevelopment from 1 to 2.
//  This will enable the needed UI/function calls for your to proceed with your assignment.
static public class AssignmentConfiguration
{
    //public const int PartOfAssignmentThatIsInDevelopment = 1;    
    public const int PartOfAssignmentThatIsInDevelopment = 2;
}

/*

In this part of the assignment you are challenged to expand on the functionality that you have already created.  
    You are being challenged to save, load and manage multiple parties.
    You are being challenged to identify each party via a string name (a member of the Party class).

To aid you in this challenge, the UI has been altered.  

    The load button has been replaced with a drop down list.  
    When this load party drop down list is changed, LoadPartyDropDownChanged(string selectedName) will be called.  
    When this drop down is created, it will be populated with the return value of GetListOfPartyNames().

    GameStart() is called when the program starts.

    For quality of life, a new SavePartyButtonPressed() has been provided to you below.

    An new/delete button has been added, you will also find below NewPartyButtonPressed() and DeletePartyButtonPressed()

Again, you are being challenged to develop the ability to save and load multiple parties.
    This challenge is different from the previous.
    In the above challenge, what you had to develop was much more directly named.
    With this challenge however, there is a much more predicate process required.
    Let me ask you,
        What do you need to program to produce the saving, loading and management of multiple parties?
        What are the variables that you will need to declare?
        What are the things that you will need to do?  
    So much of development is just breaking problems down into smaller parts.
    Take the time to name each part of what you will create and then, do it.

Good luck, journey well.
*/

enum SignifierId
{
    LastUsedIndex = 1,
    IndexAndName
}

static public class AssignmentPart2
{
    static int last_idx_used_;
    static List<string> party_list_;
    static LinkedList<NameAndIndex> name_idx_list_;
    const string kIndexFile = "Indices.txt";

    static public void GameStart()
    {
        name_idx_list_ = new LinkedList<NameAndIndex>();

        if (File.Exists(Application.dataPath + Path.DirectorySeparatorChar + kIndexFile))
        {
            StreamReader sr = new StreamReader(Application.dataPath + Path.DirectorySeparatorChar + kIndexFile);

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] csv = line.Split(',');
                int signifier = int.Parse(csv[0]);

                if (signifier == (int)SignifierId.LastUsedIndex)
                {
                    last_idx_used_ = int.Parse(csv[1]);
                }
                else if (signifier == (int)SignifierId.IndexAndName)
                {
                    name_idx_list_.AddLast(new NameAndIndex(int.Parse(csv[1]), csv[2]));
                }
            }
            sr.Close();
        }

        party_list_ = new List<string>();
        foreach (NameAndIndex item in name_idx_list_)
        {
            party_list_.Add(item.name);
        }

        GameContent.RefreshUI();
    }

    static public List<string> GetListOfPartyNames()
    {
        //return new List<string>() {
        //    "sample 1",
        //    "sample 2",
        //    "sample 3"
        //};
        return party_list_;
    }

    static public void LoadPartyDropDownChanged(string selectedName)
    {
        GameContent.partyCharacters.Clear();

        int index_to_load = -1;
        foreach (NameAndIndex item in name_idx_list_)
        {
            if (item.name == selectedName)
            {
                index_to_load = item.index;
            }
        }

        StreamReader sr = new StreamReader(Application.dataPath + Path.DirectorySeparatorChar + index_to_load + ".txt");
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            string[] csv = line.Split(',');
            int game_data_id = int.Parse(csv[0]);
            if (game_data_id == (int)GameDataId.CharacterDataId)
            {
                GameContent.partyCharacters.AddLast(new PartyCharacter(int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3]), int.Parse(csv[4]), int.Parse(csv[5]), int.Parse(csv[6])));
            }
            else if (game_data_id == (int)GameDataId.EquipmentDataId)
            {
                GameContent.partyCharacters.Last.Value.equipment.AddLast(int.Parse(csv[1]));
            }
        }
        sr.Close();
        GameContent.RefreshUI();
    }

    static public void LoadPartyFromSharedFriend(LinkedList<string> data)
    {
        //copied from LoadPartyDropDownChanged()
        GameContent.partyCharacters.Clear();
        foreach(string line in data)
        {
            string[] csv = line.Split(',');
            int game_data_id = int.Parse(csv[1]); //bumps up 1 to skip signifier
            if (game_data_id == (int)GameDataId.CharacterDataId)
            {
                GameContent.partyCharacters.AddLast(new PartyCharacter(int.Parse(csv[2]), int.Parse(csv[3]), int.Parse(csv[4]), int.Parse(csv[5]), int.Parse(csv[6]), int.Parse(csv[7])));
            }
            else if (game_data_id == (int)GameDataId.EquipmentDataId)
            {
                GameContent.partyCharacters.Last.Value.equipment.AddLast(int.Parse(csv[2]));
            }
        }
        GameContent.RefreshUI();
    }

    static public void SavePartyButtonPressed()
    {
        bool does_name_exist = false;
        foreach (NameAndIndex item in name_idx_list_)
        {
            if (item.name == GameContent.GetPartyNameFromInput())
            {
                SaveParty(Application.dataPath + Path.DirectorySeparatorChar + item.index + ".txt");
                does_name_exist = true;
                break;
            }
        }
        if (!does_name_exist)
        {
            last_idx_used_++;
            SaveParty(Application.dataPath + Path.DirectorySeparatorChar + last_idx_used_ + ".txt");
            name_idx_list_.AddLast(new NameAndIndex(last_idx_used_, GameContent.GetPartyNameFromInput()));
        }
        
        GameContent.RefreshUI();

        Debug.Log(">>> Saving to " + Application.dataPath + Path.DirectorySeparatorChar + "Party.txt");
        SaveIndexManagementFile();
    }

    static public void NewPartyButtonPressed()
    {
        Debug.Log(">>> Creating new...");
        SaveIndexManagementFile();
    }

    static public void DeletePartyButtonPressed()
    {

    }

    static public void SaveIndexManagementFile()
    {
        StreamWriter sw = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + kIndexFile);
        sw.WriteLine((int)SignifierId.LastUsedIndex + "," + last_idx_used_);
        foreach (NameAndIndex item in name_idx_list_)
        {
            sw.WriteLine((int)SignifierId.IndexAndName + "," + item.index + "," + item.name);
        }
        sw.Close();
    }

    static public void SaveParty(string file_name)
    {
        StreamWriter sw = new StreamWriter(file_name);
        foreach (PartyCharacter pc in GameContent.partyCharacters)
        {
            sw.WriteLine((int)GameDataId.CharacterDataId + "," + pc.classID + "," + pc.health + "," + pc.mana + "," + pc.strength + "," + pc.agility + "," + pc.wisdom);
            foreach (int item in pc.equipment)
            {
                sw.WriteLine((int)GameDataId.EquipmentDataId + "," + item);
            }
        }
        sw.Close();
    }

    static public void SendOnScreenPartyToServerForSharing(NetworkedClient client)
    {
        LinkedList<string> data = new LinkedList<string>();
        foreach (PartyCharacter pc in GameContent.partyCharacters)
        {
            data.AddLast((int)GameDataId.CharacterDataId + "," + pc.classID + "," + pc.health + "," + pc.mana + "," + pc.strength + "," + pc.agility + "," + pc.wisdom);
            foreach (int item in pc.equipment)
            {
                data.AddLast((int)GameDataId.EquipmentDataId + "," + item);
            }
            Debug.Log("> data is: " + data.Last);
        }
        client.SendMessageToHost(ClientToServerSignifiers.kPartyTransferDataStart + "");
        foreach (string item in data)
        {
            client.SendMessageToHost(ClientToServerSignifiers.kPartyTransferData + ","+item);
        }
        client.SendMessageToHost(ClientToServerSignifiers.kPartyTransferDataEnd + "");
    }
}

#endregion

public class NameAndIndex
{
    public string name;
    public int index;

    public NameAndIndex(int index, string name)
    {
        this.name = name;
        this.index = index;
    }
}
