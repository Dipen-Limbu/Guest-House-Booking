using System;
using System.Collections.Generic;
using System.Linq;

namespace Guest_House.DTOs.Billing
{
    /// <summary>
    /// Constants and allowed enum-like values for Billing and Invoice tables
    /// </summary>
    public static class BillingConstants
    {
        public static readonly string[] AllowedChargeTypes = new[]
        {
            "room_charge",
            "room_order",
            "minibar",
            "laundry",
            "damage",
            "extra_bed",
            "service_fee",
            "other"
        };

        public static readonly string[] AllowedInvoiceStatuses = new[]
        {
            "unpaid",
            "partially_paid",
            "paid",
            "cancelled"
        };

        public static readonly string[] AllowedInvoiceItemTypes = new[]
        {
            "room",
            "room_order",
            "laundry",
            "minibar",
            "damage",
            "extra_bed",
            "service",
            "other"
        };

        public static bool IsValidChargeType(string? chargeType)
        {
            return !string.IsNullOrWhiteSpace(chargeType) &&
                   AllowedChargeTypes.Contains(chargeType.Trim().ToLowerInvariant());
        }

        public static bool IsValidInvoiceStatus(string? status)
        {
            return !string.IsNullOrWhiteSpace(status) &&
                   AllowedInvoiceStatuses.Contains(status.Trim().ToLowerInvariant());
        }

        public static bool IsValidInvoiceItemType(string? itemType)
        {
            return !string.IsNullOrWhiteSpace(itemType) &&
                   AllowedInvoiceItemTypes.Contains(itemType.Trim().ToLowerInvariant());
        }

        /// <summary>
        /// Maps an expense charge type to an allowed invoice item type
        /// </summary>
        public static string MapChargeTypeToItemType(string chargeType)
        {
            return chargeType.Trim().ToLowerInvariant() switch
            {
                "room_charge" => "room",
                "room_order" => "room_order",
                "laundry" => "laundry",
                "minibar" => "minibar",
                "damage" => "damage",
                "extra_bed" => "extra_bed",
                "service_fee" => "service",
                _ => "other"
            };
        }
    }
}
