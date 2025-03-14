using SDL2;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestGame.GameObjects;
using TestGame.GameObjects.Textures;

namespace TestGame.Scenes {
    public class BalatroScene : Scene {
        private const int Margin = 1;
        private const int Spacing = 1;
        private const int Scale = 2;
        private const int CardHeight = 190;
        private const int CardWidth = 142;

        private const int RankLength = 13;
        private const int SuitLength = 4;

        private readonly Color _errorColor = new() { A = 255, R = 255, G = 32, B = 10 };
        private GameContext _context;
        
        private Rect[,]? _cardFaceRects;
        private Sprite32? _cardFaces;

        private Rect[,]? _cardRects;
        private Sprite32? _cards;
        
        private Random _rand;
        private Rect _destination;

        private Button nextRankButton;
        private Button previousRankButton;

        private Button nextSuitButton;
        private Button previousSuitButton;

        private int sX = 0;
        private int sY = 0;


        // Card { Margin: 1, Spacing: 1 }
        // Card Faces { Margin: ?, Spacing: ?, W: 142, H: 190 }

        public BalatroScene(GameContext context) : base(context) {
            _context = context;
            _cardFaceRects = null;
            _rand = new();
            _cardFaces = null;
            _cards = null;

            _destination = new Rect() { X = 10, Y = 10, W = CardWidth * Scale, H = CardHeight * Scale };

            nextRankButton = new(context);
            previousRankButton = new(context);
            nextSuitButton = new(context);
            previousSuitButton = new(context);
        }

        private static int _iteration = 0;
        private (int, int) GetNextRandomPair(int lastX, int lastY, int maxX = 13, int maxY = 4) {
            if (_iteration++ % 60 == 0) {
                return (_rand.Next(0, maxX), _rand.Next(0, maxY));
            }
            return (lastX, lastY);
        }

        private void AssertError(int result) {
            if (result > 0) {
                RenderText($"Error: {SDL.GetError()}", 5, 5, _errorColor);
            }
        }

        public override string Name => "Balatro";

        public override void Cleanup() {
        }

        private void CreateCardStack() {
            string texts = "W:\\resources\\textures\\2x";
            
            _cardFaces = new(_context.RendererPtr, 1846, 760, Path.Combine(texts, "8BitDeck.png"));
            _cards = new(_context.RendererPtr, 994, 950, Path.Combine(texts, "Enhancers.png"));

            _cardRects = new Rect[5, 7];
            for (int cY = 0; cY < 5; cY++) {
                for (int cX = 0; cX < 7; cX++) {
                    _cardRects[cY, cX] = new Rect() {
                        X = (cX * CardWidth) + Margin + Spacing,
                        Y = (cY * CardHeight) + Margin + Spacing,
                        W = CardWidth,
                        H = CardHeight
                    };
                }
            }

            _cardFaceRects = new Rect[SuitLength, RankLength];
            for (int cfY = 0; cfY < SuitLength; cfY++) {
                for (int cfX = 0; cfX < RankLength; cfX++) {
                    _cardFaceRects[cfY, cfX] = new Rect() {
                        X = (cfX * CardWidth) + Margin + Spacing,
                        Y = (cfY * CardHeight) + Margin + Spacing,
                        W = CardWidth,
                        H = CardHeight
                    };
                }
            }
        }

        private void CreateButtonStack() {
            Color shadowyColor = new() { A = 128, B = 0, G = 0, R = 0 };
            const bool shadowEnabled = true;
            const int shadowDepth = 5;
            previousRankButton.State = ButtonState.Default;
            previousRankButton.Width = 48;
            previousRankButton.Height = 32;
            previousRankButton.X = _destination.X + _destination.W + 10;
            previousRankButton.Y = ((_destination.Y + _destination.H) / 2) - (previousRankButton.Height / 2) - 2;
            previousRankButton.Text = "<";
            previousRankButton.ShadowEnabled = shadowEnabled;
            previousRankButton.ShadowColor = shadowyColor;
            previousRankButton.ShadowDepth = shadowDepth;
            previousRankButton.Click += PreviousRankButton_Click;

            nextRankButton.State = ButtonState.Default;
            nextRankButton.Width = 48;
            nextRankButton.Height = 32;
            nextRankButton.X = _destination.X + _destination.W + 10;
            nextRankButton.Y = ((_destination.Y + _destination.H) / 2) + (nextRankButton.Height / 2) + 2;
            nextRankButton.Text = ">";
            nextRankButton.ShadowEnabled = shadowEnabled;
            nextRankButton.ShadowColor = shadowyColor;
            nextRankButton.ShadowDepth = shadowDepth;
            nextRankButton.Click += NextRankButton_Click;

            previousSuitButton.State = ButtonState.Default;
            previousSuitButton.Width = 48;
            previousSuitButton.Height = 32;
            previousSuitButton.X = ((_destination.X + _destination.W) / 2) - (previousSuitButton.Width / 2) - 2;
            previousSuitButton.Y = _destination.Y + _destination.H + 10;
            previousSuitButton.Text = "^";
            previousSuitButton.ShadowEnabled = shadowEnabled;
            previousSuitButton.ShadowColor = shadowyColor;
            previousSuitButton.ShadowDepth = shadowDepth;
            previousSuitButton.Click += PreviousSuitButton_Click;

            nextSuitButton.State = ButtonState.Default;
            nextSuitButton.Width = 48;
            nextSuitButton.Height = 32;
            nextSuitButton.X = ((_destination.X + _destination.W) / 2) + (nextSuitButton.Width / 2) + 2;
            nextSuitButton.Y = _destination.Y + _destination.H + 10;
            nextSuitButton.Text = "V";
            nextSuitButton.ShadowEnabled = shadowEnabled;
            nextSuitButton.ShadowColor = shadowyColor;
            nextSuitButton.ShadowDepth = shadowDepth;
            nextSuitButton.Click += NextSuitButton_Click;


            AddGameObject(nextRankButton);
            AddGameObject(previousRankButton);
            AddGameObject(nextSuitButton);
            AddGameObject(previousSuitButton);
        }

        private void NextSuitButton_Click(object? sender, MouseButtonEvent e) {
            sY++;
            if (sY > SuitLength - 1) {
                sY = SuitLength - 1;
            }
        }

        private void PreviousSuitButton_Click(object? sender, MouseButtonEvent e) {
            sY--;
            if (sY < 0) {
                sY = 0;
            }
        }

        private void NextRankButton_Click(object? sender, MouseButtonEvent e) {
            sX++;
            if (sX > RankLength - 1) {
                sX = RankLength - 1;
            }
        }

        private void PreviousRankButton_Click(object? sender, MouseButtonEvent e) {
            sX--;
            if (sX < 0) {
                sX = 0;
            }
        }

        public override void Initialize() {
            CreateCardStack();
            CreateButtonStack();
        }

        public override void Draw() {
            base.Draw();

            // Random Sprite Logic, Iterate through Deck
            
            if (_cardFaceRects is null || _cardRects is null) {
                RenderText("Unable to load Sprites", 5, 5, _errorColor);
                return;
            }

            Rect cardBack = _cardRects[0, 1];
            Rect src = _cardFaceRects[sY, sX];
            if (_cardFaces is null || _cards is null) {
                RenderText($"Unable to slice Sprite at {src}", 5, 5, _errorColor);
                return;
            }
            
            int result = SDL.RenderCopy(RendererPtr, _cards.TexturePtr, ref cardBack, ref _destination);
            AssertError(result);
            result = SDL.RenderCopy(RendererPtr, _cardFaces.TexturePtr, ref src, ref _destination);
            AssertError(result);
            
        }
    }
}
