using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace VehiclesMod.Screens
{
    class VehicleSetupScreen : NewGuiMenu
    {
        public override string Name { get { return "Vehicles"; } }

        GlobalPoint3D point;

        public VehicleSetupScreen(ITMGame game, ITMPlayer player, GlobalPoint3D p)
            : base(game, player)
        {
            point = p;
        }

        protected override void InitWindows(Texture2D backTexture)
        {
            base.InitWindows(backTexture);
            InitMainContainer();
        }

        void InitMainContainer()
        {
            var screenRect = canvas.WinRect;
            canvas.OffsetMin.X = -300;
            canvas.OffsetMin.Y = -100;
            canvas.OffsetMax.X = 300;
            canvas.OffsetMax.Y = 150;

            int x = 120;
            int y = 110;
            int w = 200;
            int w2 = 320;
            int h = 34;
            int g = 4;
            var items = 13;
            int h1 = h * items + g * (items - 1);
            float scale = 0.6f;

            var container = new Window(null, x, y, w + 1 + w2, h1) { Name = "mainContainer" };
            container.Colors = Window.TransparentColorProfile;
            canvas.AddChild(container);

            Window win;
            TextBox tbox;

            x = y = 0;
            win = tbox = new TextBox("Vehicle:", x, y, w, h, scale);
            win.Colors = Colors.LabelColors;
            container.AddChild(win);
            win = tbox = new TextBox("Some Car", x + w + 1, y, w2, h, scale);
            win.Colors = Colors.ButtonColors;
            container.AddChild(win);
            y += h + g;

            win = tbox = new TextBox("Type:", x, y, w, h, scale);
            win.Colors = Colors.LabelColors;
            container.AddChild(win);
            win = tbox = new TextBox("Some Type", x + w + 1, y, w2, h, scale);
            win.Colors = Colors.ButtonColors;
            container.AddChild(win);
            y += h + g;

            win = tbox = new TextBox("Etc:", x, y, w, h, scale);
            win.Colors = Colors.LabelColors;
            container.AddChild(win);
            win = tbox = new TextBox("Etc", x + w + 1, y, w2, h, scale);
            win.Colors = Colors.ButtonColors;
            container.AddChild(win);
            y += h + g;
        }

        #region Input Handlers

        #endregion
    }
}
