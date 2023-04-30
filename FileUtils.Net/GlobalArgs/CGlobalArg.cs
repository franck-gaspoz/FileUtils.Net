using FileUtils.Net.Services.Text;
using FileUtils.Net.SharedModels;

using Microsoft.Extensions.Configuration;

namespace FileUtils.Net.GlobalArgs;

sealed class CGlobalArg : GlobalArg
{
    public ValidFileInfo FileInfo { get; private set; }

#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
    public CGlobalArg(
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        IConfiguration config,
        Texts texts) : base("c", config, texts, 1)
    {
    }

    protected override void Initialize()
        => FileInfo = new(_parameters[0], _texts);
}

