#pragma once
#include <vector>
#include <string>
#include <filesystem>

class FileArchiver
{
public:
	std::vector<std::string> trn_filenames;

	__declspec(dllexport) void CopytrnFiles2Archive(std::vector<std::filesystem::path> trn_filenames, std::string newdrive);
	__declspec(dllexport) void RemoveFilesFromStorage(std::vector<std::filesystem::path> trn_filenames, std::string newdrive);
	__declspec(dllexport) std::vector<std::string> ParseString(std::string s, std::string delimiter);
	__declspec(dllexport) int GettrnFiles(std::string site, std::string start_date, std::string end_date);
};
