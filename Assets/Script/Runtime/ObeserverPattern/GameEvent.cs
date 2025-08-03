using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NPCTurnFinishedEvent
{
    public int rewardEarned; 
}

public struct ShowDialogueEvent
{
    public string dialogueText;
}

public struct HideDialogueEvent {}

public struct UpdateNPCMoodEvent
{
    public NPC.NPCController.MoodState newMood;
}

public struct RequestNPCSpawnEvent
{
    public QuestData questData;
}

public struct RequestNPCQuitEvent
{
    public QuestData questData;
}

public struct PotionGivenToNPCEvent
{
    public Potion potion;
}

public struct QuestAboutToStartEvent
{
    public QuestData questData;
}

public struct PotionDisposedEvent {}

public struct BrewSuccessEvent { }

public struct InventoryChangedEvent {}