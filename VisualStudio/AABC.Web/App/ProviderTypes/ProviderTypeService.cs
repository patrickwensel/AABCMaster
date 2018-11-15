using AABC.Domain2.Providers;
using AABC.Web.App.ProviderTypes.Models;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Web.App.ProviderTypes
{

    public interface IProviderTypeService
    {
        IEnumerable<ProviderTypeDTO> GetProviderTypes();
        IEnumerable<ProviderSubTypeDTO> GetProviderSubTypes(int providerTypeId);
        InsertSaveOperationResult InsertProviderSubType(ProviderSubTypeDTO providerSubType);
        InsertSaveOperationResult SaveProviderSubType(ProviderSubTypeDTO providerSubType);
        OperationResult DeleteProviderSubType(int providerSubTypeId);
    }

    public class OperationResult
    {
        public bool Success { get; set; }
    }

    public class InsertSaveOperationResult : OperationResult
    {
        public ProviderSubTypeDTO Data { get; set; }
    }

    public class ProviderTypeService : IProviderTypeService
    {
        private readonly Data.V2.CoreContext Context;

        public ProviderTypeService(Data.V2.CoreContext context)
        {
            Context = context;
        }

        public IEnumerable<ProviderTypeDTO> GetProviderTypes()
        {
            var data = Context.ProviderTypes
                .OrderBy(m => m.Code)
                .ToList();
            return Mapper.ToProviderTypeDTO(data);
        }

        public IEnumerable<ProviderSubTypeDTO> GetProviderSubTypes(int providerTypeId)
        {
            var data = Context.ProviderSubTypes
                .Where(m => m.ParentTypeID == providerTypeId)
                .OrderBy(m => m.Code)
                .ToList()
                .Select(m => Mapper.ToProviderSubTypeDTO(m))
                .ToList();
            foreach (var d in data)
            {
                d.IsBeingUsed = Context.Providers.Any(m => m.ProviderSubTypeID == d.Id);
            }
            return data;
        }

        public InsertSaveOperationResult InsertProviderSubType(ProviderSubTypeDTO providerSubType)
        {
            var providerSubTypeDb = new ProviderSubType()
            {
                ParentTypeID = providerSubType.ProviderTypeId
            };
            Mapper.Map(providerSubType, providerSubTypeDb);
            Context.ProviderSubTypes.Add(providerSubTypeDb);
            Context.SaveChanges();
            return new InsertSaveOperationResult
            {
                Success = true,
                Data = Mapper.ToProviderSubTypeDTO(providerSubTypeDb)
            };
        }

        public InsertSaveOperationResult SaveProviderSubType(ProviderSubTypeDTO providerSubType)
        {
            var providerSubTypeDb = Context.ProviderSubTypes.SingleOrDefault(m => m.ID == providerSubType.Id);
            if (providerSubTypeDb == null)
            {
                return new InsertSaveOperationResult { Success = false };
            }
            Mapper.Map(providerSubType, providerSubTypeDb);
            Context.SaveChanges();
            return new InsertSaveOperationResult
            {
                Success = true,
                Data = Mapper.ToProviderSubTypeDTO(providerSubTypeDb)
            };
        }

        public OperationResult DeleteProviderSubType(int providerSubTypeId)
        {
            var subType = Context.ProviderSubTypes.SingleOrDefault(m => m.ID == providerSubTypeId);
            if (subType == null)
            {
                return new OperationResult { Success = false };
            }
            Context.Entry(subType).Collection(b => b.Providers).Load();
            Context.ProviderSubTypes.Remove(subType);
            Context.SaveChanges();
            return new OperationResult { Success = true };
        }

        static class Mapper
        {
            public static void Map(ProviderSubTypeDTO providerSubType, ProviderSubType providerSubTypeDb)
            {
                providerSubTypeDb.Code = providerSubType.Code;
                providerSubTypeDb.Name = providerSubType.Name;
            }

            public static IEnumerable<ProviderSubTypeDTO> ToProviderSubTypeDTO(IEnumerable<ProviderSubType> data)
            {
                return data.Select(m => ToProviderSubTypeDTO(m));
            }

            public static ProviderSubTypeDTO ToProviderSubTypeDTO(ProviderSubType data)
            {
                return new ProviderSubTypeDTO
                {
                    Id = data.ID,
                    ProviderTypeId = data.ParentTypeID,
                    Code = data.Code,
                    Name = data.Name
                };
            }

            public static IEnumerable<ProviderTypeDTO> ToProviderTypeDTO(IEnumerable<ProviderType> data)
            {
                return data.Select(m => ToProviderTypeDTO(m));
            }

            public static ProviderTypeDTO ToProviderTypeDTO(ProviderType data)
            {
                return new ProviderTypeDTO
                {
                    Id = data.ID,
                    Code = data.Code,
                    Name = data.Name
                };
            }
        }
    }
}