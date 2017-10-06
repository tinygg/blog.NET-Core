using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: NeutralResourcesLanguage("zh-CN")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2243:AttributeStringLiteralsShouldParseCorrectly",
    Justification = "AssemblyInformationalVersion does not need to be a parsable version")]
internal static class RevisionClass
{
    public const string Major = "3";
    public const string Minor = "0";
    public const string Build = "1";
    public const string Revision = "557";
    public const string VersionName = null;	// "" is not valid for no version name, you have to use null if you don't want a version name (eg "Beta 1")

    public const string FullVersion = Major + "." + Minor + "." + Build + "." + Revision;//557$INSERTBRANCHPOSTFIX$$INSERTVERSIONNAMEPOSTFIX$
}