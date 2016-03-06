namespace Arwic.InfiniteFrontier
{
    public enum UnitCommandType
    {
        TargetLocation,
        TargetUnit,
        TargetPlanet,
        Aura,
        Instant
    }

    public enum UnitAbilityType
    {
        TargetLocation,
        TargetUnit,
        TargetSystem,
        TargetPlanet,
        Aura,
        Instant
    }

    public enum UnitAtlas
    {
        Coloniser,
        BattleShip,
        Corvette,
        Cruiser,
        Destroyer
    }

    public enum UnitClass
    {
        Military,
        Settler,
        Commercial
    }

    public enum UnitCargo
    {
        Civilians,
        Alloys,
        Oil,
        Food,
        Millitary
    }
}
