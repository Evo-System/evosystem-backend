# Biblioteca `evosystem-backend`

Esta documentação descreve a API da biblioteca `evosystem-backend` (compilada como `evosystem-backend.dll`). O objetivo é fornecer todas as informações necessárias para instanciar as classes e consumir os métodos que buscam dados do sistema, aplicativos, drivers e arquivos do usuário.

## Como Usar
A `evosystem-backend` é uma biblioteca de classes (.dll) .NET Framework 4.7.2. Para usá-la no frontend, você precisará instanciar a classe "Manager" específica para a funcionalidade que deseja acessar (por exemplo, `SystemInfo`, `AppManager`, etc.) e, em seguida, chamar seus métodos públicos.

A maioria dos métodos já trata exceções internamente (como `UnauthorizedAccessException` ao ler o registro ou WMI). Em caso de falha, eles geralmente retornam uma lista vazia (`List<>`) ou uma string indicando um erro (ex: "Erro: ...").

## Referência da API

Abaixo estão detalhadas todas as classes e métodos públicos disponíveis para o frontend.

-----

### 1\. SystemInfo

Fornece informações estáticas sobre o hardware e o sistema operacional da máquina.

**Classe:** `evosystem_backend.SystemInfo`
**Construtor:** `public SystemInfo()`

-----

#### `public string GetOperatingSystem()`

Recupera o nome amigável (Caption) do sistema operacional.

  * **Argumentos:** Nenhum.
  * **Retorno:** `string`
  * **Exemplo de Resposta:** `"Microsoft Windows 11 Pro"`
  * **Resposta em Caso de Erro:** `"Erro: <mensagem>"` ou `"Não encontrado"`

-----

#### `public string GetCpuName()`

Recupera o nome do processador (CPU).

  * **Argumentos:** Nenhum.
  * **Retorno:** `string`
  * **Exemplo de Resposta:** `"Intel(R) Core(TM) i9-13900K"`
  * **Resposta em Caso de Erro:** `"Erro: <mensagem>"` ou `"Não encontrado"`

-----

#### `public string GetMotherboardName()`

Recupera o nome do modelo da placa-mãe.

  * **Argumentos:** Nenhum.
  * **Retorno:** `string`
  * **Exemplo de Resposta:** `"ROG STRIX Z790-E GAMING WIFI"`
  * **Resposta em Caso de Erro:** `"Erro: <mensagem>"` ou `"Não encontrado"`

-----

#### `public string GetGpuName()`

Recupera o nome da placa de vídeo (GPU) principal.

  * **Argumentos:** Nenhum.
  * **Retorno:** `string`
  * **Notas:** O método tenta ativamente filtrar adaptadores de vídeo virtuais, meta ou remotos para focar na GPU física principal.
  * **Exemplo de Resposta:** `"NVIDIA GeForce RTX 4090"`
  * **Resposta em Caso de Erro:** `"Erro: <mensagem>"` ou `"Não encontrado"`

-----

#### `public string GetTotalRam()`

Calcula a quantidade total de memória RAM física instalada.

  * **Argumentos:** Nenhum.
  * **Retorno:** `string`
  * **Notas:** O valor é retornado como uma string já formatada em Gigabytes (GB), arredondada para duas casas decimais.
  * **Exemplo de Resposta:** `"31.85 GB"`
  * **Resposta em Caso de Erro:** `"Erro ao consultar RAM: <mensagem>"` ou `"Não encontrado"`

-----

### 2\. AppManager

Gerencia a listagem de aplicativos instalados no sistema.

**Classe:** `evosystem_backend.AppManager`
**Construtor:** `public AppManager()`

-----

#### `public List<InstalledApp> GetInstalledApps()`

Recupera uma lista de aplicativos instalados, lendo a chave `Uninstall` do registro do Windows.

  * **Argumentos:** Nenhum.
  * **Retorno:** `List<InstalledApp>`
  * **Notas:** Atualmente, lê apenas de `HKEY_LOCAL_MACHINE`. Pode requerer privilégios de administrador para listar todos os apps. Se ocorrer um erro de permissão ou outro, uma lista vazia será retornada.

#### Estrutura de Resposta: `InstalledApp`

Esta é a classe que será retornada na lista.

```csharp
public class InstalledApp
{
    // Nome de exibição do aplicativo (ex: "Google Chrome")
    public string DisplayName { get; set; }

    // Data da instalação (ex: "20231027")
    // O método preenche com "N/A" se a data não for encontrada no registro.
    public string InstallDate { get; set; }
}
```

-----

### 3\. DriverManager

Gerencia a listagem de drivers de dispositivo instalados.

**Classe:** `evosystem_backend.DriverManager`
**Construtor:** `public DriverManager()`

-----

#### `public List<DriverInfo> GetDrivers()`

Recupera uma lista de drivers assinados (consulta WMI `Win32_PnPSignedDriver`).

  * **Argumentos:** Nenhum.
  * **Retorno:** `List<DriverInfo>`
  * **Notas:** Em caso de falha na consulta WMI, retorna uma lista vazia.

#### Estrutura de Resposta: `DriverInfo`

Esta é a classe que será retornada na lista.

```csharp
public class DriverInfo
{
    // Nome amigável do dispositivo (ex: "Realtek High Definition Audio")
    public string DeviceName { get; set; }

    // Versão do driver instalada (ex: "10.0.19041.1")
    // O método preenche com "N/A" se for nulo.
    public string DriverVersion { get; set; }

    // Fabricante do driver (ex: "Realtek")
    // O método preenche com "N/A" se for nulo.
    public string Manufacturer { get; set; }
}
```

-----

### 4\. StartupManager

Gerencia a listagem de aplicativos configurados para iniciar com o Windows.

**Classe:** `evosystem_backend.StartupManager`
**Construtor:** `public StartupManager()`

-----

#### `public List<StartupApp> GetStartupApps()`

Recupera uma lista de aplicativos das chaves "Run" do registro, tanto do usuário atual (`HKEY_CURRENT_USER`) quanto da máquina local (`HKEY_LOCAL_MACHINE`).

  * **Argumentos:** Nenhum.
  * **Retorno:** `List<StartupApp>`
  * **Notas:** Em caso de erro (ex: permissão), a lista pode estar incompleta (por exemplo, contendo apenas os apps do usuário atual).

#### Estrutura de Resposta: `StartupApp`

Esta é a classe que será retornada na lista.

```csharp
public class StartupApp
{
    // Nome da entrada no registro (ex: "OneDrive")
    public string Name { get; set; }

    // Caminho completo para o executável (ex: "C:\Program Files\Microsoft OneDrive\OneDrive.exe")
    public string FilePath { get; set; }

    // Indica a origem da entrada.
    // true = Veio de HKEY_CURRENT_USER (apenas para o usuário atual)
    // false = Veio de HKEY_LOCAL_MACHINE (para todos os usuários)
    public bool IsFromCurrentUser { get; set; }
}
```

-----

### 5\. FileManager

Fornece métodos para pesquisar arquivos grandes no sistema.

**Classe:** `evosystem_backend.FileManager`
**Construtor:** `public FileManager()`

-----

#### `public List<FileInfo> FindLargeFilesQuickScan(long minSizeInBytes)`

Executa uma varredura rápida por arquivos grandes nas pastas de perfil do usuário (Documentos, Vídeos, Músicas, Imagens e Desktop).

  * **Argumentos:**
      * `long minSizeInBytes`: O tamanho mínimo em bytes que um arquivo deve ter para ser incluído na lista. (Ex: `1048576` para 1 MB).
  * **Retorno:** `List<FileInfo>`

-----

#### `public List<FileInfo> FindLargeFiles(string startDirectory, long minSizeInBytes)`

Executa uma varredura completa e recursiva em um diretório específico fornecido.

  * **Argumentos:**
      * `string startDirectory`: O caminho da pasta onde a busca deve começar (ex: `"C:\"` ou `"D:\Meus Documentos"`).
      * `long minSizeInBytes`: O tamanho mínimo em bytes que um arquivo deve ter.
  * **Retorno:** `List<FileInfo>`
  * **Notas:** Este método pode demorar dependendo do diretório. Erros de acesso a pastas (como `UnauthorizedAccessException`) são capturados e ignorados, continuando a varredura em outras pastas.

#### Estrutura de Resposta: `System.IO.FileInfo`

Ambos os métodos retornam uma lista de `FileInfo`, que é uma classe interna do .NET. As propriedades mais relevantes para a UI são:

```csharp
// (Classe do System.IO)
public class FileInfo
{
    // Nome do arquivo com extensão (ex: "meu_video.mp4")
    public string Name { get; }

    // Caminho completo do arquivo (ex: "C:\Users\Joao\Videos\meu_video.mp4")
    public string FullName { get; }

    // Tamanho do arquivo em bytes (ex: 53924820)
    public long Length { get; }

    // Data da última modificação
    public DateTime LastWriteTime { get; }

    // (Existem outras propriedades, mas estas são as principais)
}
```

-----

### 6\. WingetManager

Verifica aplicativos que podem ser atualizados usando o Gerenciador de Pacotes do Windows (Winget).

**Classe:** `evosystem_backend.WingetManager`
**Construtor:** `public WingetManager()`

-----

#### `public List<string> ListUpgradableApps()`

Executa o comando `winget list --upgrade-available` e retorna a saída.

  * **Argumentos:** Nenhum.

  * **Retorno:** `List<string>`

  * **Notas Importantes:**

    1.  **Dependência Externa:** Este método **exige** que o cliente `winget.exe` (Gerenciador de Pacotes do Windows) esteja instalado na máquina do usuário e acessível no `PATH` do sistema. Se o `winget` não for encontrado ou falhar, o método retornará uma lista vazia.
    2.  **Formato do Retorno:** O retorno **NÃO** é um objeto formatado. É uma lista de `string`, onde cada string é uma linha da saída bruta do console do `winget`. O código apenas remove as linhas de cabeçalho ("Nome", "----").
    3.  **Parsing no Frontend:** O frontend será responsável por fazer o *parsing* (divisão) de cada string para extrair as colunas desejadas.

  * **Exemplo de item na lista de retorno:**

    ```
    "Microsoft.PowerToys 0.64.0 0.64.1 winget"
    ```

    O frontend precisará dividir esta string (provavelmente por espaços múltiplos) para obter o Nome (`Microsoft.PowerToys`), Versão Atual (`0.64.0`), Versão Disponível (`0.64.1`), etc.

