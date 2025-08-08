using System;
using System.Reflection;

[assembly: AssemblyVersion(RWF.RWFMod.Version)]
[assembly: AssemblyFileVersion(RWF.RWFMod.Version)]
[assembly: AssemblyInformationalVersion(RWF.RWFMod.Version)]

[assembly: Github("Bknibb", "RoundsWithFriends")]

[AttributeUsage(AttributeTargets.Assembly)]
public class GithubAttribute : Attribute
{
    public string Owner { get; }
    public string Repo { get; }

    public GithubAttribute(string owner, string repo)
    {
        Owner = owner;
        Repo = repo;
    }
}
