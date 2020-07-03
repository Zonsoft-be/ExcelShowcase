using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models
{
    public static class KnownNames
    {
        public const string InvoicesSheetName = "Invoices";
        public const string OrganisationsSheetName = "Organisations";
        public const string PaymentTermsSheetName = "PaymentTerms";
        public const string AppConfigSheetName = "AppConfig";

        public const string InvoiceTag = "{78B7A83F-F9E2-4336-83BB-14C51B9EF709}";
        public const string OrganisationTag = "{344583ED-FAEC-495E-AE45-FAD59F457AF2}";
        public const string PaymentTermTag = "{4F810B0E-2575-4FC9-BBE6-587F2EB59772}";
        public const string AppConfigTag = "{98DE017C-B3A6-4365-877C-B672CB70F3EE}";

        public const string ValidationRangeOrganisations = "ValidationRange.Organisations";
        public const string ValidationRangePaymentTerms = "ValidationRange.PaymentTerms";
        public const string ValidationRangeBooleans = "ValidationRange.Booleans";
    }
}
