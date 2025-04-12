using System.Numerics;
using Nova2D.Engine.Core;
using Nova2D.Engine.ECS;
using Nova2D.Engine.Graphics;
using Nova2D.Engine.UI;

namespace Nova2D.Demo
{
    public class MyGame : NovaGame
    {
        private Entity? _rotatingEntity;
        private UILabel? _statusLabel;

        public override void OnLoad()
        {
            LoadTexture("test", "Assets/Textures/test.png");
            LoadTexture("slime", "Assets/Textures/slime spritesheet calciumtrice.png");
            LoadFont("main", "Assets/Fonts/font.fnt");

            var testEntity = new Entity();
            testEntity.Add(new TransformComponent { Position = new Vector2(400, 300) });
            testEntity.Add(new SpriteComponent(Texture("test"))
            {
                Size = new Vector2(128, 128),
                Color = Vector4.One
            });

            _rotatingEntity = testEntity;
            AddEntity(testEntity);

            var slimeAnimation = new AnimationComponent { FrameTime = 0.1f, Loop = true };
            for (int i = 0; i < 4; i++)
                slimeAnimation.Frames.Add(new Rectangle(i * 32, 0, 32, 32));

            var slimeEntity = new Entity();
            slimeEntity.Add(new TransformComponent { Position = new Vector2(300, 300) });
            slimeEntity.Add(new SpriteComponent(Texture("slime"))
            {
                Size = new Vector2(32, 32),
                Color = Vector4.One
            });
            slimeEntity.Add(slimeAnimation);

            AddEntity(slimeEntity);

            var button = new UIButton("Click Me", Font("main"))
            {
                Position = new Vector2(20, 140),
                OnClick = () =>
                {
                    if (_statusLabel != null)
                        _statusLabel.Text = "Button Clicked!";
                }
            };

            _statusLabel = new UILabel("Waiting for click...", Font("main"))
            {
                Position = new Vector2(20, 180)
            };

            AddUI(button);
            AddUI(_statusLabel);
        }

        public override void OnUpdate(float deltaTime)
        {
            if (_rotatingEntity?.Get<TransformComponent>() is { } transform)
            {
                transform.Rotation += deltaTime;
            }
        }
    }
}
