using Godot;
namespace TROR.Module.Quest
{
    public partial class FirstButton : Button
    {
        [Export]
        private DataManager _dataManager;

        public override void _Ready()
        {
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
            _dataManager.SetValue("first_button_pressed", true);
            //GD.Print("first_button_pressed");
        }

        private void OnDataChanged(StringName key, Variant value)
        {
            if (key == "first_button_pressed")
                Visible = !value.AsBool();
        }

        private void OnResetRequested()
        {
            Visible = true;
        }
    }
}