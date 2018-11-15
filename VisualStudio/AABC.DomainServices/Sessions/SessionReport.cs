using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AABC.DomainServices.Sessions
{
    public class SessionReport
    {
        [JsonProperty(PropertyName = "summary")]
        public string Summary { get; set; }
        [JsonProperty(PropertyName = "behaviorsSection")]
        public BehaviorsReportSection BehaviorsSection { get; set; } = new BehaviorsReportSection();
        [JsonProperty(PropertyName = "interventionsSection")]
        public InterventionsReportSection InterventionsSection { get; set; } = new InterventionsReportSection();
        [JsonProperty(PropertyName = "reinforcersSection")]
        public ReinforcersReportSection ReinforcersSection { get; set; } = new ReinforcersReportSection();
        [JsonProperty(PropertyName = "goalsSection")]
        public GoalsReportSection GoalsSection { get; set; } = new GoalsReportSection();
        [JsonProperty(PropertyName = "barriersSection")]
        public BarriersReportSection BarriersSection { get; set; } = new BarriersReportSection();

        public static SessionReport CreateSample()
        {
            var config = SessionReportConfiguration.CreateSample();
            var behaviors = config.Fields.Where(f => f.Name == "Behaviors").Cast<MultiSelectFieldConfiguration<MultiSelectOption>>().Single().Options;
            var interventions = config.Fields.Where(f => f.Name == "Interventions").Cast<MultiSelectFieldConfiguration<MultiSelectOptionWithResponses>>().Single().Options;
            var reinforcers = config.Fields.Where(f => f.Name == "Reinforcers").Cast<MultiSelectFieldConfiguration<MultiSelectOption>>().Single().Options;
            var barriers = config.Fields.Where(f => f.Name == "Barriers").Cast<MultiSelectFieldConfiguration<MultiSelectOption>>().Single().Options;
            return new SessionReport
            {
                Summary = "This is a summary text. Lorem ipsum. More to come.",
                BehaviorsSection = new BehaviorsReportSection
                {
                    Behaviors = new List<Behavior>
                    {
                        new Behavior
                        {
                            Name = behaviors.First().Name,
                            Description = "Some random description"
                        },
                        new Behavior
                        {
                            Name = behaviors.Last().Name,
                            Description = null
                        }
                    }
                },
                InterventionsSection = new InterventionsReportSection
                {
                    Interventions = new List<Intervention>
                    {
                        new Intervention
                        {
                            Name = interventions.First().Name,
                            Response = interventions.First().Responses.First().Name
                        },
                        new Intervention
                        {
                            Name = interventions.Last().Name,
                            Response = interventions.Last().Responses.First().Name
                        }
                    }
                },
                ReinforcersSection = new ReinforcersReportSection
                {
                    Reinforcers = new List<Reinforcer>
                    {
                        new Reinforcer
                        {
                            Name = reinforcers.First().Name,
                            Description = "Some random description for reinforcer"
                        },
                        new Reinforcer
                        {
                            Name = reinforcers.Last().Name,
                            Description = null
                        }
                    }
                },
                GoalsSection = new GoalsReportSection()
                {
                    Goals = new List<Goal>
                    {
                        new Goal
                        {
                            Name = "Say Thank you",
                            Progress = "Reluctant at first, he finally said it"
                        }
                    }
                },
                BarriersSection = new BarriersReportSection
                {
                    Barriers = new List<Barrier>
                    {
                        new Barrier
                        {
                            Name = barriers.First().Name,
                            Description = "Some random description for barrier"
                        },
                        new Barrier
                        {
                            Name = barriers.Last().Name,
                            Description = null
                        }
                    }
                }
            };
        }

    }


    public abstract class ReportSection
    {
        public abstract IEnumerable<object> GetItems();
        public abstract Type GetItemType();
    }


    public class BehaviorsReportSection : ReportSection
    {
        [JsonProperty(PropertyName = "behaviors")]
        public IEnumerable<Behavior> Behaviors { get; set; } = new List<Behavior>();

        public override IEnumerable<object> GetItems()
        {
            return Behaviors.Cast<object>();
        }

        public override Type GetItemType()
        {
            return typeof(Behavior);
        }
    }


    public class InterventionsReportSection : ReportSection
    {
        [JsonProperty(PropertyName = "interventions")]
        public IEnumerable<Intervention> Interventions { get; set; } = new List<Intervention>();

        public override IEnumerable<object> GetItems()
        {
            return Interventions.Cast<object>();
        }

        public override Type GetItemType()
        {
            return typeof(Intervention);
        }
    }


    public class ReinforcersReportSection : ReportSection
    {
        [JsonProperty(PropertyName = "reinforcers")]
        public IEnumerable<Reinforcer> Reinforcers { get; set; } = new List<Reinforcer>();

        public override IEnumerable<object> GetItems()
        {
            return Reinforcers.Cast<object>();
        }

        public override Type GetItemType()
        {
            return typeof(Reinforcer);
        }
    }


    public class GoalsReportSection : ReportSection
    {
        [JsonProperty(PropertyName = "goals")]
        public IEnumerable<Goal> Goals { get; set; } = new List<Goal>();

        public override IEnumerable<object> GetItems()
        {
            return Goals.Cast<object>();
        }

        public override Type GetItemType()
        {
            return typeof(Goal);
        }
    }


    public class BarriersReportSection : ReportSection
    {
        [JsonProperty(PropertyName = "barriers")]
        public IEnumerable<Barrier> Barriers { get; set; } = new List<Barrier>();

        public override IEnumerable<object> GetItems()
        {
            return Barriers.Cast<object>();
        }

        public override Type GetItemType()
        {
            return typeof(Barrier);
        }
    }


    public abstract class BaseElement
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }


    public abstract class GenericElement : BaseElement
    {
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        public override string ToString()
        {
            var str = base.ToString();
            if (!string.IsNullOrEmpty(Description))
            {
                str += $" ({Description})";
            }
            return str;
        }
    }


    [DisplayName("Behaviors")]
    public class Behavior : GenericElement { }


    [DisplayName("Reinforcers")]
    public class Reinforcer : GenericElement { }


    [DisplayName("Barriers")]
    public class Barrier : GenericElement { }


    [DisplayName("Interventions")]
    public class Intervention : GenericElement
    {
        [JsonProperty(PropertyName = "response")]
        public string Response { get; set; }

        public override string ToString()
        {
            var str = base.ToString();
            if (!string.IsNullOrEmpty(Response))
            {
                str += " - Response: " + Response;
            }
            return str;
        }
    }


    [DisplayName("Goals")]
    public class Goal : BaseElement
    {
        [JsonProperty(PropertyName = "progress")]
        public string Progress { get; set; }

        public override string ToString()
        {
            var str = base.ToString();
            if (!string.IsNullOrEmpty(Progress))
            {
                str += " - Progress: " + Progress;
            }
            return str;
        }
    }

}

//- Summary(long text w/ validation similar to current notes validation (regex based as part of the template metadata?))
//- Behaviors(multi-select, optional)
//	- Checked(on)
//	- Description(text, optional)
//- Interventions(multi-select, optional)
//	- Checked
//	- Response(dropdown, required)
//	- Description(text, optional)
//- Reinforcers(multi-select, optional)
//	- Checked
//	- Description(text, optional)
//- Goals(multiple, free-form, require at least one)
//	- Goal Name(short text, required)
//	- Progress(long text, required w/ validation requrement: regex based? different from summary validation)
//- Barriers(multi-select, optional)
//	- Checked
//	- Description(text, optional)