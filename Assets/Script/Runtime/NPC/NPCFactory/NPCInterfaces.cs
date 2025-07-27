namespace NPC
{
    public interface INPCFactory
    {
        NPCController Create(QuestData quest);
    }
}