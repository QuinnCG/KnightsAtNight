using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quinn.UI
{
	[RequireComponent(typeof(UIDocument))]
	public class DeathMenu : MonoBehaviour
	{
		[SerializeField]
		private float TierFontSize = 30f, EntryFontSize = 22f;

		private Label _surviveTime, _surviveWaves, _slainCount, _soldiersSummoned, _spellsCast, _favoriteCard;
		private Button _retryButton, _quitButton, _submitButton;
		private TextField _nameField;
		private ListView _leaderboard;

		private void Awake()
		{
			var root = GetComponent<UIDocument>().rootVisualElement;

			_surviveTime = root.Q<Label>("time");
			_surviveWaves = root.Q<Label>("waves");
			_slainCount = root.Q<Label>("slain");
			_soldiersSummoned = root.Q<Label>("soldiers");
			_spellsCast = root.Q<Label>("spells");
			_favoriteCard = root.Q<Label>("favorite");

			_retryButton = root.Q<Button>("retry");
			_quitButton = root.Q<Button>("quit");
			_submitButton = root.Q<Button>("submit");

			_nameField = root.Q<TextField>("name");
			_leaderboard = root.Q<ListView>("leaderboard");

			var gm = GameManager.Instance;
			_surviveTime.text = "Survived for " + $"{(Time.time - gm.AttemptStartTime) / 60f:0.00} minutes".Color("yellow");
			_surviveWaves.text = "Survived for " + $"{gm.WavesSurvived}x waves".Color("yellow");
			_slainCount.text = "Slain " + $"{gm.EnemiesSlain}x vikings".Color("yellow");
			_soldiersSummoned.text = "Summoned " + $"{gm.SoldiersSummoned}x knights".Color("yellow");
			_spellsCast.text = "Cast " + $"{gm.SpellsCast}x spells".Color("yellow");
			_favoriteCard.text = "Favorite card " + GetFavoriteCard().Color("yellow");

			_retryButton.clicked += OnRetry;
			_quitButton.clicked += OnQuit;
			_submitButton.clicked += OnSubmit;
		}

		private IEnumerator Start()
		{
			yield return new WaitUntil(() => AuthenticationService.Instance.IsSignedIn);
			PopulateLeaderboard(string.Empty);
		}

		private void Update()
		{
			_submitButton.SetEnabled(!string.IsNullOrWhiteSpace(_nameField.text));
		}

		private void OnRetry()
		{
			GameManager.Instance.RestartGame();
		}

		private void OnQuit()
		{
			Application.Quit();
		}

		private async void OnSubmit()
		{
			string name = _nameField.text;

			if (!string.IsNullOrWhiteSpace(name))
			{
				await AuthenticationService.Instance.UpdatePlayerNameAsync(name);
				await LeaderboardsService.Instance.AddPlayerScoreAsync("highest-wave", GameManager.Instance.WavesSurvived);

				_nameField.SetValueWithoutNotify(string.Empty);
				PopulateLeaderboard(name);
			}
		}

		private async void PopulateLeaderboard(string playerName)
		{
			string currentTier = string.Empty;

			var result = await LeaderboardsService.Instance.GetScoresAsync("highest-wave");
			var entryList = result.Results;

			var items = new List<string>();
			int i = 1;
			int playerIndex = 0;

			foreach (var entry in entryList)
			{
				if (entry.Tier != currentTier)
				{
					currentTier = entry.Tier;
					string color = currentTier switch
					{
						"soldier" => "#a1593f",
						"captain" => "#e08524",
						"general" => "#e62b12",
						"royalty" => "#f5074b",
						_ => string.Empty
					};

					string title = currentTier;
					title = title[0].ToString().ToUpper() + title[1..];

					items.Add($"<b><size={TierFontSize}><color={color}>{title}</color></size></b>");
				}

				if (!string.IsNullOrWhiteSpace(playerName) && entry.PlayerName == playerName)
				{
					playerIndex = i - 1;
				}

				items.Add($"[{i}] {entry.PlayerName[..entry.PlayerName.IndexOf('#')]} - <color=yellow>{Mathf.RoundToInt((float)entry.Score)}x waves</color>");
				i++;
			}

			_leaderboard.itemsSource = items;
			_leaderboard.makeItem += () =>
			{
				var label = new Label();
				label.style.fontSize = EntryFontSize;
				return label;
			};
			_leaderboard.bindItem += (element, index) =>
			{
				var label = element as Label;
				label.text = items[index];
			};
			_leaderboard.RefreshItems();

			if (!string.IsNullOrWhiteSpace(playerName))
			{
				_leaderboard.ScrollToItem(playerIndex);
			}
		}

		private string GetFavoriteCard()
		{
			int highest = 0;
			string name = string.Empty;

			foreach (var entry in GameManager.Instance.CardUses)
			{
				if (entry.Value > highest)
				{
					highest = entry.Value;
					name = entry.Key.Title;
				}
			}

			return name;
		}
	}
}
