using AABC.Domain.General;
using System.Collections.Generic;

namespace AABC.Data.Mappings
{
    public static class GeneralMappings
    {

        public static GuardianRelationship GuardianRelationship(Models.GuardianRelationship entity) {
            var model = new GuardianRelationship();
            model.ID = entity.ID;
            model.Name = entity.RelationshipName;
            return model;
        }

        public static IEnumerable<GuardianRelationship> GuardianRelationships(IEnumerable<Models.GuardianRelationship> entities) {
            var models = new List<GuardianRelationship>();
            foreach (var entity in entities) {
                models.Add(GeneralMappings.GuardianRelationship(entity));
            }
            return models;
        }

        public static GeneralLanguage GeneralLanguage(Models.CommonLanguage entity) {
            var model = new GeneralLanguage();
            model.ID = entity.ID;
            model.Code = entity.Code;
            model.Description = entity.Description;
            return model;
        } 


        public static GeneralLanguage GeneralLanguage(Models.ISO639_2_Lang entity) {
            var model = new GeneralLanguage();
            model.ID = entity.ID;
            model.Code = entity.ISO_639_1;
            model.Description = entity.EnglishName;
            return model;
        }


        public static IEnumerable<GeneralLanguage> GeneralLanguages(IEnumerable<Models.CommonLanguage> entities) {
            var models = new List<GeneralLanguage>();
            foreach (var entity in entities) {
                models.Add(GeneralMappings.GeneralLanguage(entity));
            }
            return models;
        }

    }
}
