﻿using System;
using System.Numerics;
using Accountant.Gui.Helper;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Textures;

namespace Accountant.Gui.Timer.Cache;

public struct CacheObject
{
    public string   Name;
    public DateTime DisplayTime;
    public string?  DisplayString;
    public uint     Icon;
    public float    IconOffset;
    public ColorId  Color;
    public Action?  TooltipCallback;

    private void DrawIcon()
    {
        if (!Dalamud.GetIcon(Icon, out var icon))
            return;

        if (IconOffset == 0)
        {
            ImGui.Image(icon.Handle, Vector2.One * ImGui.GetTextLineHeight());
        }
        else
        {
            var offset = Vector2.One * IconOffset;
            var size   = Vector2.One - offset;
            ImGui.Image(icon.Handle, Vector2.One * ImGui.GetTextLineHeight(), offset, size);
        }

        ImGui.SameLine();
    }

    public void Draw(DateTime now)
    {
        if (Accountant.Config.HideDisabled && Color == ColorId.DisabledText)
            return;

        using var color   = ImGuiRaii.PushColor(ImGuiCol.Text, Color.Value());
        using var spacing = ImGuiRaii.PushStyle(ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemSpacing / 2);
        DrawIcon();
        ImGui.Selectable(Name);
        var tooltip = ImGui.IsItemHovered();
        spacing.Pop();

        if (DisplayString != null)
        {
            var width = ImGui.CalcTextSize(DisplayString).X;
            ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X - width);
            ImGui.Text(DisplayString);
        }
        else if (DisplayTime > now)
        {
            var display = TimerWindow.TimeSpanString(DisplayTime - now);
            var width   = ImGui.CalcTextSize(display).X;
            ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X - width);
            ImGui.Text(display);
        }

        color.Pop();
        if (tooltip)
            TooltipCallback?.Invoke();
    }
}
