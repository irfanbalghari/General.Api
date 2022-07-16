using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Core.Enums
{
    public class Enums
    {
        public enum RequestType
        {
            InApp = 1,
            External = 2
        }
        public enum RequestStatus
        {
            NotStarted = 1,
            InProgress = 2,
            PendingFinancialAssessment = 3,
            FinancialAssessmentApproved = 4,
            FinancialAssessmentRejected = 5,
            ContractSigning = 6,
            Signed = 7,
            NotSigned = 8
        }
        public enum Category
        {
            Advertisement = 1,
            Offer = 2
        }
        public enum LeasType
        {
            Financial = 1,
            Operational = 2
        }

        public enum RecordType
        {
            Yellow = 1,
            White = 2,
            Parrot = 3
        }
        public enum BodyType
        {
            Hatchback = 1,
            Sedan = 2,
            Stationcar = 3,
            MPV = 4,
            SUV = 5,
            Cabriolet = 6,
            Coupe = 7,
            Mini = 8,
            Compact = 9,
            Van = 10,
            Roadster = 11,
            OffRoad = 12,
            Crossover = 13,
            Pickup = 14,
            GT = 15,
            Sportback = 16

        }
        public enum FuelType
        {
            Pertrol = 1,
            Diesel = 2,
            Huybrid = 3,
            EI = 4
        }
        public enum PullType
        {
            FrontWheel = 1,
            RearWheel = 2,
            FourWheel = 3
        }
        public enum TransmissionType
        {
            Manual = 1,
            Automatic = 2
        }

        public enum CarStatus
        {
            InProgress = 1,
            Completed = 2
        }

        public enum WarrantyServiceType
        {
            Basic = 1,
            Comfort = 2,
            ComfortPlus = 3
        }
        public enum CaseFileStatus
        {
            New = 0,
            Pending = 1,
            Rejected = 2,
            Deleted = 3,
            Signed = 4,
            Completed = 5
        }
        public enum ApprovalType
        {
            Risika = 1,
            Monthio = 2,
            RKI = 3,
            AML = 4,
            Documentations = 4
        }

        public enum CustomerStatus
        {
            InActive = 1,
            Active = 2
        }
    }
}
