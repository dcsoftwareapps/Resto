using System.Collections.Frozen;

namespace Resto.Contracts;

public static class Permissions
{
    public const string PlatformRemoteLogin = "platform.remote_login";
    public const string PlatformDevices = "platform.devices";
    public const string OrdersView = "orders.view";
    public const string PosTake = "pos.take";
    public const string PosCancel = "pos.cancel";
    public const string PosDiscount = "pos.discount";
    public const string PosReprint = "pos.reprint";
    public const string PosTransfer = "pos.transfer";
    public const string CashView = "cash.view";
    public const string CashCharge = "cash.charge";
    public const string CashWithdraw = "cash.withdraw";
    public const string CashCut = "cash.cut";
    public const string CashRefund = "cash.refund";
    public const string CashSettings = "cash.settings";
    public const string KitchenView = "kitchen.view";
    public const string KitchenManage = "kitchen.manage";
    public const string InventoryView = "inventory.view";
    public const string InventoryAdjust = "inventory.adjust";
    public const string MenuView = "menu.view";
    public const string MenuEdit = "menu.edit";
    public const string PurchasingRequest = "purchasing.request";
    public const string PurchasingView = "purchasing.view";
    public const string PurchasingBuy = "purchasing.buy";
    public const string PurchasingSuppliers = "purchasing.suppliers";
    public const string PromosManage = "promos.manage";
    public const string InvoicingIssue = "invoicing.issue";
    public const string InvoicingGlobal = "invoicing.global";
    public const string InvoicingCancel = "invoicing.cancel";
    public const string InvoicingSettings = "invoicing.settings";
    public const string ReportsView = "reports.view";
    public const string OnlineManage = "online.manage";
    public const string AuditView = "audit.view";
    public const string SecurityManage = "security.manage";
    public const string UsersManage = "users.manage";
    public const string SettingsManage = "settings.manage";
    public const string BranchesManage = "branches.manage";
    public const string BillingManage = "billing.manage";

    public static IReadOnlySet<string> All { get; } = new[]
    {
        PlatformRemoteLogin,
        PlatformDevices,
        OrdersView,
        PosTake,
        PosCancel,
        PosDiscount,
        PosReprint,
        PosTransfer,
        CashView,
        CashCharge,
        CashWithdraw,
        CashCut,
        CashRefund,
        CashSettings,
        KitchenView,
        KitchenManage,
        InventoryView,
        InventoryAdjust,
        MenuView,
        MenuEdit,
        PurchasingRequest,
        PurchasingView,
        PurchasingBuy,
        PurchasingSuppliers,
        PromosManage,
        InvoicingIssue,
        InvoicingGlobal,
        InvoicingCancel,
        InvoicingSettings,
        ReportsView,
        OnlineManage,
        AuditView,
        SecurityManage,
        UsersManage,
        SettingsManage,
        BranchesManage,
        BillingManage,
    }.ToFrozenSet(StringComparer.Ordinal);
}
