using SDL2;
using TestGame.GameObjects;

namespace TestGame.Scenes
{
    /// <inheritdoc />
    public class PauseScene(GameContext context, string name) : Scene(context, name) {

        private const string PauseText = "PAUSED";
        private const string PauseTextLong = "HURRY UP, WILL YA!?";

        #region Overrides of Scene
        
        private TimeSpan _lastTs;
        private long _lastTime;
        private bool _triggerWarning;
        private string? _currentText;
        
        /// <inheritdoc />
        public override void Initialize() {
            // TODO: There is a better way to handle this shit. Figure it out.
            var clockCon = context with { Rect = new() { X = Width / 2, Y = 10, W = 64, H = 12 } };
            var buttonCon = context with { Rect = new() { X = Width / 2 - 75, Y = Height / 2 + 25, W = 75, H = 25 } };

            SceneEnter += PauseScene_SceneEnter;
            SceneLeave += PauseScene_SceneLeave;

            var resumeButton = new MenuButton(buttonCon) {
                Text = "Resume"
            };
            
            var quitButton = new MenuButton(buttonCon with { Rect = buttonCon.Rect with { Y = (int)resumeButton.Y + resumeButton.Height + 4 } }) {
                Text = "Quit"
            };
            
            resumeButton.Click += ResumeButton_Click;
            quitButton.Click += QuitButton_Click;
            
            AddGameObject(new Clock(clockCon, @"C:\Windows\Fonts\consola.ttf", false));
            AddGameObject(resumeButton);
            AddGameObject(quitButton);
        }

        private void PauseScene_SceneLeave(object? sender, EventArgs.SceneEventArgs sea) {
            _triggerWarning = false;
            _lastTime = 0;
            ResourceManager.Get<SoundEffect>("audio/shutdown").Play();
        }
        
        private void PauseScene_SceneEnter(object? sender, EventArgs.SceneEventArgs sea) {
            _triggerWarning = false;
            _lastTime = DateTime.Now.ToBinary();
            ResourceManager.Get<SoundEffect>("audio/pause").Play();
        }

        private void ResumeButton_Click(object? sender, MouseButtonEvent e) {
            Button? button = ((Button?)sender);
            if (button is null) return;
            Core.Instance.TogglePause();
        }

        private void QuitButton_Click(object? sender, MouseButtonEvent e) {
            Button? button = ((Button?)sender);
            if (button is null) return;
            Core.Instance?.Stop();
        }

        public override void Cleanup() {
            foreach (var gameObject in GameObjects) {
                RemoveGameObject(gameObject);
            }
        }

        private int cx, cy;
        private Size cs;
        private Color foreground = new() { A = 255, B = 255, G = 255, R = 255 };
        
        /// <inheritdoc />
        public override void Draw() {    
            base.Draw();

            if (_currentText is null || _currentText.Length == 0) return;

            cs = MeasureString(GetFont("consolas", 24), _currentText!);
            cx = (Width / 2) - (cs.Width / 2);
            cy = (Height / 2) - (cs.Height / 2);

            RenderText(_currentText, 24, cx, cy, foreground);
            
        }

        public override void Update(Event e) {
            base.Update(e);
            _lastTs = DateTime.Now - DateTime.FromBinary(_lastTime);
            _triggerWarning = _lastTs.TotalSeconds >= 5;
            _currentText = _triggerWarning ? PauseTextLong : PauseText;
        }

        #endregion
    }
}
