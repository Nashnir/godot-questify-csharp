using Godot;

namespace TROR.Module.Quest
{
    public partial class SecondButton : Button
    {
        [Export]
        private DataManager _dataManager;

        [Export]
        private string _value = "";

        public override void _Ready()
        {
            Visible = false;

            if (_dataManager == null)
            {
                GD.PushError($"{Name}: DataManager is not assigned.");
                return;
            }

            Pressed += OnPressed;
            _dataManager.DataChanged += OnDataChanged;
            _dataManager.ResetRequested += OnResetRequested;
        }

        private void OnPressed()
        {
            _dataManager.SetValue("second_button_pressed", _value);
            //GD.Print("second_button_pressed");
        }

        private void OnDataChanged(StringName key, Variant value)
        {
            if (key == "first_button_pressed")
                Visible = true;

            if (key == "second_button_pressed")
                Visible = false;
        }

        private void OnResetRequested()
        {
            Visible = false;
        }
    }
}