using System.Text;
using B4DLib;
using Godot;
using Nakama;

namespace NakamaDemo
{

    public class Controller : Node
    {
        private const string Key = "nakama_godot_demo";
        private const string Host = "192.168.99.101"; //default 127.0.0.1
        private static readonly PackedScene EnemyScene = ResourceLoader.Load<PackedScene>("res://Enemy.tscn");
        
        

        public static string MatchId = string.Empty;

        public override async void _Ready()
        {
            NakamaServer.InitNakamaClient(Host,Key);

            //Signals
            NakamaServer.Connected += () => GetNode<Player>("Player").Id = NakamaServer.GetUserId();
            NakamaServer.MatchStateReceived += NakamaServerOnMatchStateReceived;
            NakamaServer.ChannelMessageReceived += message => GD.Print(message.Content);
            NakamaServer.MatchPresenceReceived += NakamaServerOnMatchPresenceReceived;
            
            
            await NakamaServer.AuthenticateAsync("test1@test.com", "password");
            await NakamaServer.OpenConnectionAsync();

            //Join Match
            var initMatchRpc = await NakamaServer.CallRpcAsync("init_matches");
            GD.Print(initMatchRpc.Payload);

            var twoMatchRpc = await NakamaServer.CallRpcAsync("two_match");

            var twoMatch = await NakamaServer.JoinMatchAsync(twoMatchRpc.Payload);

            foreach (var presence in twoMatch.Presences)
            {
                if (presence.UserId.Equals(GetNode<Player>("Player").Id)) return;
                CreateEnemyInstance(presence.UserId);
            }
            MatchId = twoMatchRpc.Payload;

            #region Commented

            ////Send Match State
            //await NakamaServer.SendMatchStateAsync(twoMatch.Id, 1, "{\"Two Match\" : \"50,10\"}");

            ////Chat 
            //await NakamaServer.JoinChatAsync("1vs1");
            //await NakamaServer.SendChatMsgAsync("1vs1" , "{\"msg\" : \"Hi\"}");




            ////Storage
            //await NakamaServer.StorageWriteAsync(new IApiWriteStorageObject[]{new WriteStorageObject()
            //{
            //    Collection = "Scores",
            //    Key = "1vs1",
            //    PermissionRead = (int) NakamaServer.ReadPermission.OwnerRead,
            //    PermissionWrite = (int) NakamaServer.WritePermission.OwnerWrite,
            //    Value = "{ \"Score\": 27 }",
            //    Version = ""
            //}});

            //var storageReadResult = await NakamaServer.StorageReadAsync(new IApiReadStorageObjectId[] {new StorageObjectId()
            //{
            //    Collection = "Scores",
            //    Key = "1vs1",
            //    UserId = NakamaServer.NakamaSession.UserId,
            //    Version = ""
            //}});
            //foreach (var storageObject in storageReadResult.Objects)
            //{
            //    GD.Print(storageObject.Value);
            //}

            ////Matchmaker 
            //var matchmakerProperties = new MatchmakerProperties();
            //await NakamaServer.CreateMatchmakerAsync(matchmakerProperties); // Returns a ticket



            //NakamaServer.MatchmakerMatched += async matched =>
            //{
            //    GD.Print("This Is From matchmaker matched status");
            //    var matchmakerMatch = await NakamaServer.JoinMatchAsync(matched);
            //    await NakamaServer.SendMatchStateAsync(matchmakerMatch.Id, 2, "{\"Another World\":\"Cyprus Match\"}");
            //};
            #endregion

        }


        private void NakamaServerOnMatchStateReceived(IMatchState state)
        {
           GD.Print(Encoding.UTF8.GetString(state.State));
        }


        private void NakamaServerOnMatchPresenceReceived(IMatchPresenceEvent presences)
        {
            foreach (var join in presences.Joins)
            {
                CreateEnemyInstance(join.UserId);
                GD.Print(join.Username, " Joined !");
            }

            foreach (var leave in presences.Leaves)
            {
                RemoveEnemyInstance(leave.UserId);
                GD.Print(leave.Username, " Left !");

            }
        }

        //Create a new enemy instance
        private void CreateEnemyInstance(string id)
        {
            if (id.Equals(NakamaServer.GetUserId())) return;
            var enemyInstance =  (Enemy) EnemyScene.Instance();
            enemyInstance.Id = id;
            GetNode<Node2D>("Enemies").AddChild(enemyInstance);
        }

        private void RemoveEnemyInstance(string id)
        {
            var enemies = GetNode<Node2D>("Enemies").GetChildren();
            foreach (Enemy enemy in enemies)
            {
                if (enemy.Id.Equals(id))
                {
                    enemy.QueueFree();
                }
            }
        }



    }
}
