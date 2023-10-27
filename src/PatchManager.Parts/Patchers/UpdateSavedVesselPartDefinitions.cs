﻿using KSP.Game;
using KSP.Game.Flow;
using KSP.Game.Load;
using KSP.Sim.State;
using PatchManager.Shared;

namespace PatchManager.Parts.Patchers;

public class UpdateSavedVesselPartDefinitions : FlowAction
{
    private LoadGameData _loadGameData;

    public UpdateSavedVesselPartDefinitions(LoadGameData loadGameData) : base("Updating saved vessel part definitions")
    {
        _loadGameData = loadGameData;
    }
    public override void DoAction(Action resolve, Action<string> reject)
    {
        if (_loadGameData.SavedGame.Vessels == null)
        {
            resolve();
            return;
        }
        foreach (var vessel in _loadGameData.SavedGame.Vessels)
        {
            // Lets change only a few things
            // Add modules, change resource containers
            foreach (var part in vessel.parts)
            {
                var name = part.partName;
                var def = GameManager.Instance.Game.Parts.Get(name);
                for (var i = part.PartModulesState.Count - 1; i >= 0; i--)
                {
                    var i2 = i;
                    if (def.data.serializedPartModules.All(x => x.Name != part.PartModulesState[i2].Name))
                    {
                        part.PartModulesState.RemoveAt(i);
                    }
                }
        
                foreach (var mod in def.data.serializedPartModules)
                {
                    if (part.PartModulesState.All(x => x.Name != mod.Name))
                    {
                        part.PartModulesState.Add(mod);
                    }
                }
        
                foreach (var resource in def.data.resourceContainers)
                {
                    if (!part.partState.resources.ContainsKey(resource.name))
                    {
                        part.partState.resources[resource.name] = new ContainedResourceState
                        {
                            name = resource.name,
                            storedUnits = resource.initialUnits,
                            capacityUnits = resource.capacityUnits
                        };
                    }
                }
            }
        }
        resolve();
    }
}