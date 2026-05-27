using Godot;
using Godot.Collections;

namespace TROR.Module.Quest
{
    public partial class DataManager : Node
    {
        [Signal]
        public delegate void DataChangedEventHandler(StringName key, Variant value);

        [Signal]
        public delegate void ResetRequestedEventHandler();

        private readonly Dictionary<StringName, Variant> _data = [];

        public override void _Ready()
        {
            //GD.Print($"DataManager READY: {GetPath()}");
            Questify.ConnectConditionQueryRequested(OnConditionQueryRequested);
        }

        private void OnConditionQueryRequested( string type, string key, Variant expectedValue, Resource questCondition)
        {
            //GD.Print($"Condition query on {GetPath()}: type={type}, key={key}, expected={expectedValue}");

            if (type != "variable")
                return;

            Variant currentValue = GetValue(key);
            //GD.Print($"Current value for {key}: {currentValue}");

            if (currentValue.VariantType == Variant.Type.Nil)
                return;

            if (VariantValuesMatch(currentValue, expectedValue))
            {
                //GD.Print($"Condition completed: {key} == {expectedValue}");
                Questify.SetConditionCompleted(questCondition, true);
            }
        }

        private static bool VariantValuesMatch(Variant currentValue, Variant expectedValue)
        {
            if (currentValue.VariantType != expectedValue.VariantType)
            {
                // Useful for cases where one side is StringName/string-ish.
                if ( currentValue.VariantType == Variant.Type.String || expectedValue.VariantType == Variant.Type.String ||
                    currentValue.VariantType == Variant.Type.StringName || expectedValue.VariantType == Variant.Type.StringName)
                {
                    return currentValue.AsString() == expectedValue.AsString();
                }

                return false;
            }

            return currentValue.VariantType switch
            {
                    Variant.Type.Bool => currentValue.AsBool() == expectedValue.AsBool(),
                    Variant.Type.Int => currentValue.AsInt64() == expectedValue.AsInt64(),
                    Variant.Type.Float => Mathf.IsEqualApprox( (float)currentValue.AsDouble(), (float)expectedValue.AsDouble()),
                    Variant.Type.String => currentValue.AsString() == expectedValue.AsString(),
                Variant.Type.StringName => currentValue.AsString() == expectedValue.AsString(),
                                    _ => currentValue.AsString() == expectedValue.AsString()
            };
        }

        public void SetValue(StringName key, Variant value)
        {
            //GD.Print($"DataManager SET on {GetPath()}: {key} = {value}");

            _data[key] = value;
            EmitSignal(SignalName.DataChanged, key, value);

            // Needed if Questify update polling is disabled.
            Questify.UpdateQuests();
        }

        public Variant GetValue(StringName key)
        {
            return _data.TryGetValue(key, out Variant value)
                ? value
                : default;
        }

        public void Clear()
        {
            _data.Clear();
            EmitSignal(SignalName.ResetRequested);
        }
    }
}