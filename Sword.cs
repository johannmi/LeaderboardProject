
using System.ComponentModel.DataAnnotations;
using System;
using FluentValidation;

  public enum SwordTypes 
        {
            regular,
            Poison
            
        }

    public class Sword : Item
    {
            public int OwnerLevel {get; set;}

            [EnumDataType(typeof(SwordTypes))]
            public SwordTypes SwordType { get; set; }
            public int Damage{get; set;}
      
    }

    //if the players level is too low, no poisontype
    public class LevelValidator: AbstractValidator<Sword>{
        public LevelValidator(){
            RuleFor(x=>x.OwnerLevel).GreaterThan(15).When(x=>x.SwordType == SwordTypes.Poison);
        }
    }