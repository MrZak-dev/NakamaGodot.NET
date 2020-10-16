using System.Linq;
using System.Text;
using B4DLib;
using Godot;
using Godot.Collections;
using Nakama;
using Newtonsoft.Json;

namespace NakamaDemo
{
    public class Enemy : Player
    {
        public override void _Ready()
        {
            NakamaServer.MatchStateReceived += NakamaServerOnMatchStateReceived;
            base._Ready();
        }

        private void NakamaServerOnMatchStateReceived(IMatchState matchState)
        {
            var decodedState = Encoding.UTF8.GetString(matchState.State);
            var data = JsonConvert.DeserializeObject <Dictionary<string, Vector2>>(decodedState).First();
            if (!data.Key.Equals(Id)) return;
            UpdatePosition(data.Value);
        }

        protected override void UpdatePosition(Vector2 targetPosition)
        {
            Tween.InterpolateProperty(this, "position", Position, targetPosition, 0.5f);
            Tween.Start();
        }

        public override void _Input(InputEvent @event)
        {
            return;
        }
    }
}
