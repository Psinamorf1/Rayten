using Content.Client.Administration.Managers;
using Content.Client.UserInterface.Controls;
using Content.Shared.Administration;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Network;
using Robust.Shared.Utility;

namespace Content.Client.Administration.UI.PlayerPanel;

[GenerateTypedNameReferences]
public sealed partial class PlayerPanel : FancyWindow
{
    private readonly IClientAdminManager _adminManager;

    public event Action<string>? OnUsernameCopy;
    public event Action<NetUserId?>? OnOpenNotes;
    public event Action<NetUserId?>? OnOpenBans;
    public event Action<NetUserId?>? OnAhelp;
    public event Action<string?>? OnKick;
    public event Action<string?>? OnCamera;
    public event Action<NetUserId?>? OnOpenBanPanel;
    public event Action<NetUserId?, bool>? OnWhitelistToggle;
    public event Action? OnFollow;
    public event Action? OnFreezeAndMuteToggle;
    public event Action? OnFreeze;
    public event Action? OnLogs;
    public event Action? OnDelete;
    public event Action? OnRejuvenate;

    public NetUserId? TargetPlayer;
    public string? TargetUsername;
    private bool _isWhitelisted;

    public PlayerPanel(IClientAdminManager adminManager)
    {
        RobustXamlLoader.Load(this);
        _adminManager = adminManager;

        UsernameCopyButton.OnPressed += _ => OnUsernameCopy?.Invoke(TargetUsername ?? "");
        BanButton.OnPressed += _ => OnOpenBanPanel?.Invoke(TargetPlayer);
        KickButton.OnPressed += _ => OnKick?.Invoke(TargetUsername);
        CameraButton.OnPressed += _ => OnCamera?.Invoke(TargetUsername);
        NotesButton.OnPressed += _ => OnOpenNotes?.Invoke(TargetPlayer);
        ShowBansButton.OnPressed += _ => OnOpenBans?.Invoke(TargetPlayer);
        AhelpButton.OnPressed += _ => OnAhelp?.Invoke(TargetPlayer);
        WhitelistToggle.OnPressed += _ =>
        {
            OnWhitelistToggle?.Invoke(TargetPlayer, _isWhitelisted);
            SetWhitelisted(!_isWhitelisted);
        };
        FollowButton.OnPressed += _ => OnFollow?.Invoke();
        FreezeButton.OnPressed += _ => OnFreeze?.Invoke();
        FreezeAndMuteToggleButton.OnPressed += _ => OnFreezeAndMuteToggle?.Invoke();
        LogsButton.OnPressed += _ => OnLogs?.Invoke();
        DeleteButton.OnPressed += _ => OnDelete?.Invoke();
        RejuvenateButton.OnPressed += _ => OnRejuvenate?.Invoke();
    }

    public void SetUsername(string player)
    {
        Title = Loc.GetString("player-panel-title", ("player", player));
        PlayerName.Text = Loc.GetString("player-panel-username", ("player", player));
    }

    public void SetWhitelisted(bool? whitelisted)
    {
        if (whitelisted == null)
        {
            Whitelisted.Text = null;
            WhitelistToggle.Visible = false;
        }
        else
        {
            Whitelisted.Text = Loc.GetString("player-panel-whitelisted");
            WhitelistToggle.Text = whitelisted.Value ? Loc.GetString("player-panel-true") : Loc.GetString("player-panel-false");
            WhitelistToggle.Visible = true;
            _isWhitelisted = whitelisted.Value;
        }
    }

    public void SetBans(int? totalBans, int? totalRoleBans)
    {
        // If one value exists then so should the other.
        DebugTools.Assert(totalBans.HasValue && totalRoleBans.HasValue || totalBans == null && totalRoleBans == null);

        Bans.Text = totalBans != null ? Loc.GetString("player-panel-bans", ("totalBans", totalBans)) : null;

        RoleBans.Text = totalRoleBans != null ? Loc.GetString("player-panel-rolebans", ("totalRoleBans", totalRoleBans)) : null;
    }

    public void SetNotes(int? totalNotes)
    {
        Notes.Text = totalNotes != null ? Loc.GetString("player-panel-notes", ("totalNotes", totalNotes)) : null;
    }

    public void SetSharedConnections(int sharedConnections)
    {
        SharedConnections.Text = Loc.GetString("player-panel-shared-connections", ("sharedConnections", sharedConnections));
    }

    public void SetPlaytime(TimeSpan playtime)
    {
        Playtime.Text = Loc.GetString("player-panel-playtime",
            ("days", playtime.Days),
            ("hours", playtime.Hours % 24),
            ("minutes", playtime.Minutes % (24 * 60)));
    }

    public void SetFrozen(bool canFreeze, bool frozen)
    {
        FreezeAndMuteToggleButton.Disabled = !canFreeze;
        FreezeButton.Disabled = !canFreeze || frozen;

        FreezeAndMuteToggleButton.Text = Loc.GetString(!frozen ? "player-panel-freeze-and-mute" : "player-panel-unfreeze");
    }

    public void SetAhelp(bool canAhelp)
    {
        AhelpButton.Disabled = !canAhelp;
    }

    public void SetButtons()
    {
        BanButton.Disabled = !_adminManager.CanCommand("banpanel");
        KickButton.Disabled = !_adminManager.CanCommand("kick");
        CameraButton.Disabled = !_adminManager.CanCommand("camera");
        NotesButton.Disabled = !_adminManager.CanCommand("adminnotes");
        ShowBansButton.Disabled = !_adminManager.CanCommand("banlist");
        WhitelistToggle.Disabled =
            !(_adminManager.CanCommand("whitelistadd") && _adminManager.CanCommand("whitelistremove"));
        LogsButton.Disabled = !_adminManager.CanCommand("adminlogs");
        RejuvenateButton.Disabled = !_adminManager.HasFlag(AdminFlags.Debug);
        DeleteButton.Disabled = !_adminManager.HasFlag(AdminFlags.Debug);
    }
}
