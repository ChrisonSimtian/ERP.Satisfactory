using ERP.Domain;
using SatisfactorySaveNet;
using SatisfactorySaveNet.Abstracts.Model;

namespace Satisfactory.Save;

/// <summary>
/// Parses a Satisfactory <c>.sav</c> file via the patched
/// <c>SatisfactorySaveNet</c> fork and projects the result into game-agnostic
/// domain entities. v1 returns counts + positions only; deeper property
/// extraction (recipe binding, miner→node linkage, belt endpoints) is tracked
/// as a follow-up on milestone #12.
/// </summary>
public sealed class SaveFileReader
{
    public LiveFactoryState Read(string savePath)
    {
        var save = SaveFileSerializer.Instance.Deserialize(savePath);

        var metadata = new SaveMetadata(
            SessionName: save.Header.SessionName ?? "(unknown)",
            SaveVersion: save.Header.SaveVersion,
            BuildVersion: save.Header.BuildVersion,
            PlayedTime: TimeSpan.FromSeconds(save.Header.PlayedSeconds),
            SaveDateTimeUtc: save.Header.SaveDateTimeUtc);

        if (save.Body is not BodyV8 body)
        {
            return new LiveFactoryState(
                metadata, [], [], [], [], [],
                [$"Unsupported body version: {save.Body?.GetType().Name ?? "null"}. v1.2 saves are expected to be BodyV8."]);
        }

        var resourceNodes = new List<ResourceNode>();
        var miners = new List<Miner>();
        var buildings = new List<ProductionBuilding>();
        var belts = new List<ConveyorBelt>();
        var generators = new List<PowerGenerator>();
        var warnings = new List<string>();

        foreach (var obj in body.Levels.SelectMany(l => l.Objects))
        {
            if (obj is not ActorObject actor) continue;

            var typePath = actor.TypePath ?? string.Empty;
            var reference = actor.ObjectReference?.PathName ?? string.Empty;
            var position = new Position(actor.Position.X, actor.Position.Y, actor.Position.Z);

            if (BuildingIdentifiers.MinerTier(typePath) is { } tier)
            {
                miners.Add(new Miner(reference, tier, position, ResourceNodeReference: null));
            }
            else if (BuildingIdentifiers.IsProductionBuilding(typePath))
            {
                var buildingId = new BuildingId(BuildingIdentifiers.ShortName(typePath));
                buildings.Add(new ProductionBuilding(reference, buildingId, position, Recipe: null));
            }
            else if (BuildingIdentifiers.BeltTier(typePath) is { } beltTier)
            {
                belts.Add(new ConveyorBelt(reference, beltTier, position));
            }
            else if (BuildingIdentifiers.GeneratorKind(typePath) is { } genKind)
            {
                generators.Add(new PowerGenerator(reference, genKind, position));
            }
            else if (BuildingIdentifiers.IsResourceNode(typePath))
            {
                resourceNodes.Add(new ResourceNode(reference, Resource: null, NodePurity.Unknown, position));
            }
        }

        return new LiveFactoryState(metadata, resourceNodes, miners, buildings, belts, generators, warnings);
    }
}
