using System;
using System.Diagnostics;
//css_ref %WIXSHARP_DIR%\wixsharp.dll;
using WixSharp;

public class Script
{
    static public void Main(string[] args)
    {
        string projectName = args[0];
        string projectNameExe = projectName + ".exe";
        string projectFolder = args[1];
        string binaryFolder = projectFolder + @"\" + projectName + @"\bin\Release\";
        string assemblyPath = binaryFolder + projectNameExe;
        FileVersionInfo assemblyInfo = FileVersionInfo.GetVersionInfo(assemblyPath);
        Version version = new Version(assemblyInfo.FileVersion);

        Console.WriteLine("Project name: " + projectName);
        Console.WriteLine("Project folder: " + projectFolder);
        Console.WriteLine("Binary folder: " + binaryFolder);
        Console.WriteLine("Assembly path: " + assemblyPath);
        Console.WriteLine("Version: " + version.ToString());

        Project project =
            new Project(projectName + "_" + version.ToString(),
                new Dir(new Id("INSTALL_DIR"), @"%ProgramFiles%\" + projectName,
                    new Files(binaryFolder + "*.exe"),
                    new Files(binaryFolder + "*.exe.config"),
                    new Files(binaryFolder + "*.dll"),

                    new Dir(@"%ProgramMenu%\" + projectName,
                        new ExeFileShortcut(projectName, "[INSTALL_DIR]" + projectNameExe, ""),
                        new ExeFileShortcut("Uninstall " + projectName, "[System64Folder]msiexec.exe", "/x [ProductCode]")
                    ),
                    new Dir(@"%Startup%\",
                        new ExeFileShortcut(projectName, "[INSTALL_DIR]" + projectNameExe, "")
                    )
                )
            );

        project.Version = version;
        project.GUID = new Guid("486e6547-0e79-4b25-bd39-11353b95210b");

        //project.SetNetFxPrerequisite("NETFRAMEWORK45 >='#378389'", "Please install .Net 4.5 first");
        project.ControlPanelInfo.ProductIcon = projectFolder + @"\" + projectName + @"\Resources\trafficlights.ico";
        project.ControlPanelInfo.NoModify = true;
        project.ControlPanelInfo.Manufacturer = assemblyInfo.CompanyName;

        project.UI = WUI.WixUI_Common;
        var customUI = new CommomDialogsUI();
        var prevDialog = Dialogs.WelcomeDlg;
        var nextDialog = Dialogs.VerifyReadyDlg;
        customUI.UISequence.RemoveAll(x => (x.Dialog == prevDialog && x.Control == Buttons.Next) || (x.Dialog == nextDialog && x.Control == Buttons.Back));
        customUI.On(prevDialog, Buttons.Next, new ShowDialog(nextDialog));
        customUI.On(nextDialog, Buttons.Back, new ShowDialog(prevDialog));
        project.CustomUI = customUI;

        Compiler.BuildMsi(project);
    }
}