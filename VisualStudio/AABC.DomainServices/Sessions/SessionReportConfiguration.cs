using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AABC.DomainServices.Sessions
{
    public class SessionReportConfiguration
    {
        public IEnumerable<BaseFieldConfiguration> Fields { get; set; }

        public static SessionReportConfiguration CreateSample()
        {
            //(values, as taken from client email)	
            //Summary of session. 3 sentence minimum. (3 periods)
            //•	Behaviors during session-  (dropdown or checkbox) Elopement, Hand Flapping, Aggression, Inappropriate Vocalizations, non- compliance, property destruction, Tantrum Behavior.NA
            //•	Interventions that occurred during the session: (drop down or checkbox )  Differential Reinforcement, Positive Punishment, Negative reinforcement.NA
            //•	Response to interventions: (drop down/check box)   Patient responded very well to intervention, Patient responded somewhat well to intervention,  Patient was neutral to intervention, patient condition regressed due to intervention
            //•	Rein forcers used during the session(drop down/check box) Tangibles, Edibles, Praise, Token board
            //•	Goals addressed(include progress made or lack thereof)  Free Form- need a minimum sentence length
            //•	Barriers to progress: (drop down list) Behavioral, Environmental, Peers, NA
            return new SessionReportConfiguration
            {
                Fields = new List<BaseFieldConfiguration> {
                    new TextFieldConfiguration {
                        Name = "Summary"
                    },
                    new MultiSelectFieldConfiguration<MultiSelectOption> {
                        Name = "Behaviors",
                        Options = new List<MultiSelectOption> {
                            new MultiSelectOption { Name = "Elopement" },
                            new MultiSelectOption { Name = "Hand Flapping"},
                            new MultiSelectOption { Name = "Aggression"},
                            new MultiSelectOption { Name = "Inappropriate Vocalizations"},
                            new MultiSelectOption { Name = "Non-compliance"},
                            new MultiSelectOption { Name = "Property Destruction"},
                            new MultiSelectOption { Name = "Tantrum Behavior"}
                        }
                    },
                    new MultiSelectFieldConfiguration<MultiSelectOptionWithResponses>("interventions"){
                        Name = "Interventions",
                        Options = new List<MultiSelectOptionWithResponses>{
                            new MultiSelectOptionWithResponses{
                                Name = "Differential Reinforcement",
                                Responses = new List<MultiSelectOption>{
                                    new MultiSelectOption { Name = "Patient responded very well to intervention"},
                                    new MultiSelectOption { Name = "Patient responded somewhat well to intervention"},
                                    new MultiSelectOption { Name = "Patient was neutral to intervention"},
                                    new MultiSelectOption { Name = "Patient condition regressed due to intervention" }
                                }
                            },
                            new MultiSelectOptionWithResponses {
                                Name = "Positive Punishment",
                                Responses = new List<MultiSelectOption>{
                                    new MultiSelectOption { Name = "Patient responded very well to intervention"},
                                    new MultiSelectOption { Name = "Patient responded somewhat well to intervention"},
                                    new MultiSelectOption { Name = "Patient was neutral to intervention"},
                                    new MultiSelectOption { Name = "Patient condition regressed due to intervention" }
                                }
                            },
                            new MultiSelectOptionWithResponses {
                                Name = "Negative reinforcement",
                                Responses = new List<MultiSelectOption>{
                                    new MultiSelectOption { Name = "Patient responded very well to intervention"},
                                    new MultiSelectOption { Name = "Patient responded somewhat well to intervention"},
                                    new MultiSelectOption { Name = "Patient was neutral to intervention"},
                                    new MultiSelectOption { Name = "Patient condition regressed due to intervention" }
                                }
                            }
                        }
                    },
                    new MultiSelectFieldConfiguration<MultiSelectOption> {
                        Name = "Reinforcers",
                        Options = new List<MultiSelectOption> {
                            new MultiSelectOption { Name = "Tangibles"},
                            new MultiSelectOption { Name = "Edibles"},
                            new MultiSelectOption { Name = "Praise"},
                            new MultiSelectOption { Name = "Token board" }
                        }
                    },
                    new CustomFieldConfiguration("Goals"){
                        Name = "Goals"
                    },
                    new MultiSelectFieldConfiguration<MultiSelectOption>
                    {
                        Name = "Barriers",
                        Options = new List<MultiSelectOption> {
                            new MultiSelectOption { Name = "Behavioral"},
                            new MultiSelectOption { Name = "Environmental"},
                            new MultiSelectOption { Name = "Peers"},
                            new MultiSelectOption { Name = "NA"}
                        }
                    },
                }
            };
        }
    }


    public abstract class BaseFieldConfiguration
    {
        public string Name { get; set; }
        public string ControlType { get; private set; }

        protected BaseFieldConfiguration() : this(null) { }

        protected BaseFieldConfiguration(string controlType)
        {
            ControlType = !string.IsNullOrEmpty(controlType) ? NormalizeControlType(controlType) : GetControlType();
        }

        private string GetControlType()
        {
            var name = GetType().Name;
            var index = name.IndexOf('`');
            return NormalizeControlType(index == -1 ? name : name.Substring(0, index));
        }

        private static string NormalizeControlType(string controlType)
        {
            var name = Regex.Replace(controlType, "FieldConfiguration$", string.Empty);
            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }
    }

    public class TextFieldConfiguration : BaseFieldConfiguration
    {

    }

    public class MultiSelectOption
    {
        public string Name { get; set; }
    }

    public class MultiSelectFieldConfiguration<TOptions> : BaseFieldConfiguration
        where TOptions : MultiSelectOption
    {
        public MultiSelectFieldConfiguration() : base() { }

        public MultiSelectFieldConfiguration(string controlType) : base(controlType) { }

        public IEnumerable<TOptions> Options { get; set; }
    }

    public class CustomFieldConfiguration : BaseFieldConfiguration
    {
        public CustomFieldConfiguration(string controlType) : base(controlType)
        {
            if (string.IsNullOrEmpty(controlType)) throw new ArgumentNullException(nameof(controlType));
        }
    }

    public class MultiSelectOptionWithResponses : MultiSelectOption
    {
        public IEnumerable<MultiSelectOption> Responses { get; set; }
    }
}
