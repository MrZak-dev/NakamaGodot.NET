using System.Linq;
using B4DLib;
using Godot;
using Godot.Collections;
using Newtonsoft.Json;

namespace NakamaDemo
{
    public class Player : Position2D
    {
        protected Tween Tween;
        protected Vector2 TargetPosition = new Vector2();
        public string Id;
        public override void _Ready()
        {
            Tween = GetNode<Tween>("Tween");
        }

        protected virtual async void UpdatePosition(Vector2 targetPosition)
        {
            Tween.InterpolateProperty(this, "position", Position, targetPosition, 0.5f);
            Tween.Start();
            var userPosition = new Dictionary<string, Vector2>
            {
                {Id, targetPosition}
            };
            
            var state = JsonConvert.SerializeObject(userPosition);

            await NakamaServer.SendMatchStateAsync(Controller.MatchId, 1,state);
        }

        public override void _Input(InputEvent @event)
        {
            if (!@event.IsActionPressed("click_tap")) return;
            UpdatePosition(GetGlobalMousePosition());
        }

    }
}
