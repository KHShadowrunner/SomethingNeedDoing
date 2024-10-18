﻿using ECommons;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Policy;
using ECommons;
namespace SomethingNeedDoing.Misc.Commands;

public class CharacterStateCommands
{
    internal static CharacterStateCommands Instance { get; } = new();

    public List<string> ListAllFunctions()
    {
        var methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        var list = new List<string>();
        foreach (var method in methods.Where(x => x.Name != nameof(ListAllFunctions) && x.DeclaringType != typeof(object)))
        {
            var parameterList = method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}{(p.IsOptional ? " = " + (p.DefaultValue ?? "null") : "")}");
            list.Add($"{method.ReturnType.Name} {method.Name}({string.Join(", ", parameterList)})");
        }
        return list;
    }

    public bool IsPlayerAvailable() => Player.Interactable && !GenericHelpers.IsOccupied();

    public unsafe bool HasStatus(string statusName)
    {
        statusName = statusName.ToLowerInvariant();
        var sheet = Svc.Data.GetExcelSheet<Sheets.Status>()!;
        var statusIDs = sheet
            .Where(row => row.Name.RawString.Equals(statusName, StringComparison.InvariantCultureIgnoreCase))
            .Select(row => row.RowId)
            .ToArray()!;

        return HasStatusId(statusIDs);
    }

    public unsafe bool HasStatusId(params uint[] statusIDs)
    {
        var statusID = Svc.ClientState.LocalPlayer!.StatusList
            .Select(se => se.StatusId)
            .ToList().Intersect(statusIDs)
            .FirstOrDefault();

        return statusID != default;
    }

    public uint GetStatusStackCount(uint statusID) => Svc.ClientState.LocalPlayer?.StatusList.FirstOrDefault(x => x.StatusId == statusID)?.StackCount ?? 0;
    public float GetStatusTimeRemaining(uint statusID) => Svc.ClientState.LocalPlayer?.StatusList.FirstOrDefault(x => x.StatusId == statusID)?.RemainingTime ?? 0;
    public uint GetStatusSourceID(uint statusID) => Svc.ClientState.LocalPlayer?.StatusList.FirstOrDefault(x => x.StatusId == statusID)?.SourceId ?? 0;

    public bool GetCharacterCondition(int flagID, bool hasCondition = true) => hasCondition ? Svc.Condition[flagID] : !Svc.Condition[flagID];

    public string GetCharacterName(bool includeWorld = false)
        => Svc.ClientState.LocalPlayer == null ? "null"
        : includeWorld ? $"{Svc.ClientState.LocalPlayer.Name}@{Svc.ClientState.LocalPlayer.HomeWorld.GameData!.Name}"
        : Svc.ClientState.LocalPlayer.Name.ToString();

    public bool IsInZone(int zoneID) => Svc.ClientState.TerritoryType == zoneID;

    public bool IsLocalPlayerNull() => Svc.ClientState.LocalPlayer == null;

    public bool IsPlayerDead() => Svc.ClientState.LocalPlayer!.IsDead;

    public bool IsPlayerCasting() => Svc.ClientState.LocalPlayer!.IsCasting;

    public unsafe bool IsLevelSynced() => UIState.Instance()->PlayerState.IsLevelSynced == 1;

    public unsafe bool IsMoving() => AgentMap.Instance()->IsPlayerMoving == 1;

    public bool IsPlayerOccupied() => GenericHelpers.IsOccupied();

    public unsafe uint GetGil() => InventoryManager.Instance()->GetGil();

    public uint GetClassJobId() => Svc.ClientState.LocalPlayer!.ClassJob.Id;
    public uint GetHP() => Svc.ClientState.LocalPlayer?.CurrentHp ?? 0;
    public uint GetMaxHP() => Svc.ClientState.LocalPlayer?.MaxHp ?? 0;
    public uint GetMP() => Svc.ClientState.LocalPlayer?.CurrentMp ?? 0;
    public uint GetMaxMP() => Svc.ClientState.LocalPlayer?.MaxMp ?? 0;
    public uint GetCurrentWorld() => Svc.ClientState.LocalPlayer?.CurrentWorld.Id ?? 0;
    public uint GetHomeWorld() => Svc.ClientState.LocalPlayer?.HomeWorld.Id ?? 0;

    public float GetPlayerRawXPos(string character = "")
    {
        if (!character.IsNullOrEmpty())
        {
            unsafe
            {
                if (int.TryParse(character, out var p))
                {
                    var go = Utils.GetGameObjectFromPronounID((uint)(p + 42));
                    return go != null ? go->Position.X : -1;
                }
                else return Svc.Objects.Where(x => x.IsTargetable).FirstOrDefault(x => x.Name.ToString().Equals(character))?.Position.X ?? -1;
            }
        }
        return Svc.ClientState.LocalPlayer!.Position.X;
    }

    public float GetPlayerRawYPos(string character = "")
    {
        if (!character.IsNullOrEmpty())
        {
            unsafe
            {
                if (int.TryParse(character, out var p))
                {
                    var go = Utils.GetGameObjectFromPronounID((uint)(p + 42));
                    return go != null ? go->Position.Y : -1;
                }
                else return Svc.Objects.Where(x => x.IsTargetable).FirstOrDefault(x => x.Name.ToString().Equals(character))?.Position.Y ?? -1;
            }
        }
        return Svc.ClientState.LocalPlayer!.Position.Y;
    }

    public float GetPlayerRawZPos(string character = "")
    {
        if (!character.IsNullOrEmpty())
        {
            unsafe
            {
                if (int.TryParse(character, out var p))
                {
                    var go = Utils.GetGameObjectFromPronounID((uint)(p + 42));
                    return go != null ? go->Position.Z : -1;
                }
                else return Svc.Objects.Where(x => x.IsTargetable).FirstOrDefault(x => x.Name.ToString().Equals(character))?.Position.Z ?? -1;
            }
        }
        return Svc.ClientState.LocalPlayer!.Position.Z;
    }

    public unsafe int GetLevel(int expArrayIndex = -1)
    {
        if (expArrayIndex == -1) expArrayIndex = Svc.ClientState.LocalPlayer!.ClassJob.GameData!.ExpArrayIndex;
        return UIState.Instance()->PlayerState.ClassJobLevels[expArrayIndex];
    }

    public unsafe byte GetPlayerGC() => UIState.Instance()->PlayerState.GrandCompany;
    public unsafe int GetFCRank() => ((InfoProxyFreeCompany*)Framework.Instance()->UIModule->GetInfoModule()->GetInfoProxyById(InfoProxyId.FreeCompany))->Rank;
    public unsafe string GetFCGrandCompany() => ((InfoProxyFreeCompany*)Framework.Instance()->UIModule->GetInfoModule()->GetInfoProxyById(InfoProxyId.FreeCompany))->GrandCompany.ToString();
    public unsafe int GetFCOnlineMembers() => ((InfoProxyFreeCompany*)Framework.Instance()->UIModule->GetInfoModule()->GetInfoProxyById(InfoProxyId.FreeCompany))->OnlineMembers;
    public unsafe int GetFCTotalMembers() => ((InfoProxyFreeCompany*)Framework.Instance()->UIModule->GetInfoModule()->GetInfoProxyById(InfoProxyId.FreeCompany))->TotalMembers;

    public unsafe void RequestAchievementProgress(uint id) => FFXIVClientStructs.FFXIV.Client.Game.UI.Achievement.Instance()->RequestAchievementProgress(id);
    public unsafe uint GetRequestedAchievementProgress() => FFXIVClientStructs.FFXIV.Client.Game.UI.Achievement.Instance()->ProgressMax;
    public unsafe bool IsAchievementComplete(int id) => FFXIVClientStructs.FFXIV.Client.Game.UI.Achievement.Instance()->IsComplete(id); // requires the achievement menu to be loaded

    public unsafe uint GetCurrentBait() => PlayerState.Instance()->FishingBait;

    public unsafe ushort GetLimitBreakCurrentValue() => UIState.Instance()->LimitBreakController.CurrentUnits;
    public unsafe uint GetLimitBreakBarValue() => UIState.Instance()->LimitBreakController.BarUnits;
    public unsafe byte GetLimitBreakBarCount() => UIState.Instance()->LimitBreakController.BarCount;

    public unsafe uint GetPenaltyRemainingInMinutes() => UIState.Instance()->InstanceContent.GetPenaltyRemainingInMinutes(0);

    public unsafe byte GetMaelstromGCRank() => PlayerState.Instance()->GCRankMaelstrom;
    public unsafe byte GetFlamesGCRank() => PlayerState.Instance()->GCRankImmortalFlames;
    public unsafe byte GetAddersGCRank() => PlayerState.Instance()->GCRankTwinAdders;
    public unsafe void SetMaelstromGCRank(byte rank) => PlayerState.Instance()->GCRankMaelstrom = rank;
    public unsafe void SetFlamesGCRank(byte rank) => PlayerState.Instance()->GCRankImmortalFlames = rank;
    public unsafe void SetAddersGCRank(byte rank) => PlayerState.Instance()->GCRankTwinAdders = rank;

    public unsafe bool HasFlightUnlocked(uint territory = 0) => PlayerState.Instance()->IsAetherCurrentZoneComplete(Svc.Data.GetExcelSheet<TerritoryType>()?.GetRow(territory != 0 ? territory : Svc.ClientState.TerritoryType)?.Unknown32 ?? 0);
    public unsafe bool TerritorySupportsMounting() => Svc.Data.GetExcelSheet<TerritoryType>()?.GetRow(Player.Territory)?.Unknown32 != 0;
    public unsafe bool InSanctuary() => TerritoryInfo.Instance()->InSanctuary;

    public unsafe bool HasWeeklyBingoJournal() => PlayerState.Instance()->HasWeeklyBingoJournal;
    public unsafe bool IsWeeklyBingoExpired() => PlayerState.Instance()->IsWeeklyBingoExpired();
    public unsafe uint WeeklyBingoNumSecondChancePoints() => PlayerState.Instance()->WeeklyBingoNumSecondChancePoints;
    public unsafe int WeeklyBingoNumPlacedStickers() => PlayerState.Instance()->WeeklyBingoNumPlacedStickers;
    public unsafe int GetWeeklyBingoTaskStatus(int wonderousTailsIndex) => (int)PlayerState.Instance()->GetWeeklyBingoTaskStatus(wonderousTailsIndex);
    public unsafe uint GetWeeklyBingoOrderDataKey(int wonderousTailsIndex) => PlayerState.Instance()->WeeklyBingoOrderData[wonderousTailsIndex];
    public unsafe uint GetWeeklyBingoOrderDataType(uint wonderousTailsKey) => (Svc.Data.GetExcelSheet<WeeklyBingoOrderData>()?.GetRow(wonderousTailsKey)?.Type).GetValueOrDefault();
    public unsafe uint GetWeeklyBingoOrderDataData(uint wonderousTailsKey) => (Svc.Data.GetExcelSheet<WeeklyBingoOrderData>()?.GetRow(wonderousTailsKey)?.Data).GetValueOrDefault();
    public unsafe string GetWeeklyBingoOrderDataText(uint wonderousTailsKey) => Svc.Data.GetExcelSheet<WeeklyBingoOrderData>()?.GetRow(wonderousTailsKey)?.Text.Value?.Description ?? string.Empty;

    public bool IsAetheryteUnlocked(uint id) => Svc.AetheryteList.Any(x => x.AetheryteId == id);
    public List<uint> GetAetheryteList() => Svc.AetheryteList.Select(x => x.AetheryteId).ToList();
    public List<uint> GetAetherytesInZone(uint zoneID) => Svc.AetheryteList.Where(x => x.TerritoryId == zoneID).Select(x => x.AetheryteId).ToList();
    public string GetAetheryteName(uint aetheryteID) => Svc.AetheryteList.FirstOrDefault(x => x.AetheryteId == aetheryteID)?.AetheryteData.GameData?.PlaceName.Value?.Name ?? string.Empty;

    private static float ConvertMapMarkerToRawPosition(int pos, float scale = 100f)
    {
        var num = scale / 100f;
        var rawPosition = ((float)(pos - 1024.0) / num);
        return rawPosition;
    }
    public unsafe bool IsFriendOnline(byte* name, ushort worldId) => InfoProxyFriendList.Instance()->GetEntryByName(name, worldId)->State != InfoProxyCommonList.CharacterData.OnlineStatus.Offline;

    public unsafe float GetJobExp(uint classjob) => PlayerState.Instance()->ClassJobExperience[GenericHelpers.GetRow<ClassJob>(classjob)?.ExpArrayIndex ?? 0];
}
