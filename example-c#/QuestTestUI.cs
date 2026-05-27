using Godot;
namespace TROR.Module.Quest
{
	public partial class QuestTestUI : Control
	{
		[Export]
		private Resource _quest;

		private DataManager _dataManager;
		private Label _currentQuestLabel;
		private ItemList _objectives;

		public override async void _Ready()
		{
			_dataManager = GetNode<DataManager>("DataManager");
			_currentQuestLabel = GetNode<Label>("%CurrentQuestLabel");
			_objectives = GetNode<ItemList>("%Objectives");

			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

			Questify.ConnectQuestStarted(OnQuestStarted);
			Questify.ConnectQuestObjectiveCompleted(OnQuestObjectiveCompleted);
			Questify.ConnectQuestObjectiveAdded(OnQuestObjectiveAdded);
			Questify.ConnectQuestCompleted(OnQuestCompleted);

			StartFreshQuest();
		}

		private void OnQuestStarted(Resource newQuest)
		{
			string questName = newQuest.Get("name").AsString();
			string description = newQuest.Get("description").AsString();

			_currentQuestLabel.Text = $"{questName} - {description}";
		}

		private void OnQuestObjectiveCompleted(Resource quest, Resource objective)
		{
			_objectives.Clear();
		}

		private void OnQuestObjectiveAdded(Resource quest, Resource objective)
		{
			bool isExclusive = objective.Get("is_exclusive").AsBool();
			string description = objective.Get("name").AsString();

			_objectives.AddItem($"{(isExclusive ? "OR: " : "")}{description}");
		}

		
		private void OnQuestCompleted(Resource completedQuest)
		{
			string questName = completedQuest.Get("name").AsString();

			_currentQuestLabel.Text = $"{questName} - COMPLETED!";
			_objectives.Clear();
		}

		private void OnResetButtonPressed()
		{
			//GD.Print("RESET PRESSED");

			_objectives.Clear();
			_currentQuestLabel.Text = "";

			_dataManager.Clear();

			Questify.Clear();

			StartFreshQuest();
		}

		private void StartFreshQuest()
		{
			if (_quest == null)
			{
				GD.PushError($"{Name}: Quest resource is not assigned.");
				return;
			}

			Resource questInstance = Questify.Instantiate(_quest);

			if (questInstance == null)
			{
				GD.PushError($"{Name}: Questify.Instantiate returned null.");
				return;
			}

			Questify.StartQuest(questInstance, new Godot.Collections.Dictionary());
		}
	}
}
