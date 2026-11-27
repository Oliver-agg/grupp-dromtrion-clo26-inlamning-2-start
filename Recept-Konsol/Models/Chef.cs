namespace Recept.Models;

//En kock, den som har skapat receptet
//Bara Id och förnamn sparas (dateminimering, GDPR): beställningen säger att förnamn räcker.

public class Chef
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
}
