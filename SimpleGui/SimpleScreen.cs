using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Integration;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace SimpleGui
{
    class SimpleScreen : NewGuiMenu
    {
        struct SimpleData
        {
            public string TextBox;
            public string DataField;
            public string DropDown;
            public float SliderValue;
        }
        static SimpleData data;

        public override string Name { get { return "Simple"; } }

        public SimpleScreen(ITMGame game, ITMPlayer player)
            : base(game, player)
        {
            if (data.TextBox == null)
            {
                // one time initialization of static data
                data.TextBox = "Some Text";
                data.DataField = "Edit Me";
                data.SliderValue = 0.5f;
            }
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

            Window win;
            TextBox tbox;
            DataField df;
            DropDown dd;
            Slider slider;

            int x = 120;
            int y = 110;
            int w = 200;
            int h = 34;
            int g = 4;
            float scale = 0.6f;

            win = tbox = new TextBox("Label:", x, y, w, h, scale);
            win.Colors = Colors.LabelColors;
            canvas.AddChild(win);
            win = tbox = new TextBox(data.TextBox, x + w + 1, y, w, h, scale);
            win.Colors = Colors.ButtonColors;
            win.SetToolTip("This is a simple Textbox. It is used to display information. It cannot be edited.");
            canvas.AddChild(win);
            y += h + g;

            win = tbox = new TextBox("Datafield:", x, y, w, h, scale);
            win.Colors = Colors.LabelColors;
            canvas.AddChild(win);
            win = tbox = df = new DataField(data.DataField, x + w + 1, y, w, h, scale);
            win.Colors = Colors.DataFieldColors.Copy(new DataField.ColorProfile());
            win.Colors.BackColor = Color.Transparent;
            ((ITextInputWindow)df).OnValidateInput = ValidateDatafield;
            canvas.AddChild(win);
            y += h + g;

            win = tbox = new TextBox("Dropdown:", x, y, w, h, scale);
            win.Colors = Colors.LabelColors;
            canvas.AddChild(win);
            win = tbox = dd = new DropDown(data.DropDown, x + w + 1, y, w, h, 300, scale);
            win.Colors = Colors.ButtonColors;
            dd.PopulateList = PopulateDropDown;
            ((ITextInputWindow)dd).OnValidateInput = ValidateDropDown;
            canvas.AddChild(win);
            y += h + g;

            win = tbox = new TextBox("Slider:", x, y, w, h, scale);
            win.Colors = Colors.LabelColors;
            canvas.AddChild(win);
            win = tbox = slider = new Slider(x + w + 1, y, w, h, scale);
            win.Colors = Colors.ButtonColors;
            slider.SetValue(data.SliderValue);
            slider.DragSliderHandler += DragSlider;
            canvas.AddChild(win);
            y += h + g;
        }

        #region Input Handlers

        void ValidateDatafield(ITextInputWindow win)
        {
            data.DataField = win.Text;
        }

        void PopulateDropDown(Window win, List<string> list, string input)
        {
            list.Clear();
            list.Add("Option xxx");
            list.Add("Option yyy");
            list.Add("Option zzz");
        }

        void ValidateDropDown(ITextInputWindow win)
        {
            data.DropDown = win.Text;
        }

        void DragSlider(object sender, WindowDragEventArgs e)
        {
            data.SliderValue = (float)e.Tag;
            var percent = (int)(data.SliderValue * 100);
            ((TextBox)sender).Text = percent.ToString() + "%";
        }

        #endregion
    }
}
