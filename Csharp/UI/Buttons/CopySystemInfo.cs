using Godot;

public partial class CopySystemInfo : Button {
	public override void _Ready() {
		ButtonUp += _CopyInfo;
	}

	private void _CopyInfo() {
		string operating_system = _GetOperatingSystem();
		string software_version = _GetSoftwareVersion();
		string gpu = _GetGPU();
		string cpu = _GetCPU();
		string memory = _GetMemory();
		
		string to_copy = string.Join(" - ", [operating_system, software_version, gpu, cpu, memory]);
		
		DisplayServer.ClipboardSet(to_copy);
	}

	private string _GetOperatingSystem() {
		string str = OS.GetVersionAlias();
		return $"{OS.GetName()} {str.Substr(0, str.IndexOf(' '))}";
	}

	private string _GetSoftwareVersion() {
		return $"{ApplicationManager.SOFTWARE_NAME} {ProjectSettings.GetSetting("application/config/version")}";
	}

	private string _GetGPU() {
		return $"{RenderingServer.GetVideoAdapterName()} ({RenderingServer.GetVideoAdapterVendor()}; {OS.GetVideoAdapterDriverInfo()[1]})";
	}

	private string _GetCPU() {
		return $"{OS.GetProcessorName()} ({OS.GetProcessorCount()} threads)";
	}

	private string _GetMemory() {
		return $"{((double)OS.GetMemoryInfo()["physical"] / 1073741824.0).ToString("0.##")}GiB memory";
	}
}
