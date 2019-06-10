using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class ClientDataManager
{
    public const string DATA_FILE_NAME = "clientData.json";

    private static ClientData cache;
    private static ClientData Data
    {
        get
        {
            if (cache == null)
            {
                string filePath = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
                if (File.Exists(filePath))
                {
                    string dataAsJson = File.ReadAllText(filePath);
                    cache = JsonUtility.FromJson<ClientData>(dataAsJson);
                }
                else
                {
                    cache = new ClientData();
                }
            }

            return cache;
        }
    }

    private static void WriteClientData()
    {
        string dataAsJson = JsonUtility.ToJson(Data, true);

        string filePath = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
        File.WriteAllText(filePath, dataAsJson);
    }

    public static IEnumerable<IClient> GetClients()
    {
        return Data.clients.FindAll(p => !p.Deleted).OrderBy(x => x.Name);
    }

    public static Dictionary<char, List<Client>> GetClientsToDictionary()
    {
        return Data.clients.FindAll(p => !p.Deleted).OrderBy(x => x.Name)
                                                    .GroupBy(x => x.Name.ToLower().First())
                                                    .ToDictionary(x => x.Key, x => x.ToList());
    }

    public static IEnumerable<IClient> GetDeletedClients()
    {
        return Data.clients.FindAll(p => p.Deleted);
    }

    public static IClient GetClient(string ID)
    {
        return Data.clients.Find(p => p.ID.Equals(ID));
    }

    public static IClient AddClient(Client newClient)
    {
        Data.clients.Add(newClient);
        WriteClientData();

        return newClient;
    }

    public static void DeleteClient(string ID)
    {
        Client client = Data.clients.Find(p => p.ID.Equals(ID));
        if (client != null)
        {
            client.Deleted = true;
            WriteClientData();
        }
    }
    public static void EditClient(string ID, Client update)
    {
        Client client = Data.clients.Find(p => p.ID.Equals(ID));
        if (client != null)
        {
            client.Name = update.Name;
            client.Number = update.Number;
            client.Adress = update.Adress;
            client.Email = update.Email;
            WriteClientData();
        }
    }

    [Serializable]
    private class ClientData
    {
        public List<Client> clients;

        public ClientData()
        {
            this.clients = new List<Client>();
        }
    }

    [Serializable]
    public  class Client : IClient
    {
        [SerializeField]
        private string id;
        public string ID => id;

        [SerializeField]
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                WriteClientData();
            }
        }

        [SerializeField]
        private string number;
        public string Number
        {
            get => number;
            set
            {
                number = value;
                WriteClientData();
            }
        }
        [SerializeField]
        private string email;
        public string Email
        {
            get => email;
            set
            {
                email = value;
                WriteClientData();
            }
        }
        [SerializeField]
        private string adress;
        public string Adress
        {
            get => adress;
            set
            {
                adress = value;
                WriteClientData();
            }
        }


        [SerializeField]
        private bool deleted = false;
        public bool Deleted
        {
            get => deleted;
            set
            {
                deleted = value;
                WriteClientData();
            }
        }


        public Client()
        {
            this.id = Guid.NewGuid().ToString();
        }
    }
}
