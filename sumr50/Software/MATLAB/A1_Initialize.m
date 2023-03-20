%% A1_Initialize
% created by: J.Aho 9/22/11
% modified by: Christopher Bay 12/5/2017

% This code will set up your present working directory, add the paths for
% functions, templates and data and you can declare the directory in which
% the turbsim wind files are stored

format compact

% Set A_CD as the present working directory
A_CD = pwd;
OpenFASTpath = cd(cd('..'));
basePath = cd(cd(['..' filesep '..']));

%Add patsh of functions, templates and data to be loaded
addpath(fullfile(A_CD,'A_Functions'));
addpath(fullfile(basePath,'Analysis'));
% addpath(fullfile(basePath,'Master'));
addpath([OpenFASTpath filesep 'build' filesep 'bin']);
% addpath(fullfile(parentpath,'A_Functions/MBC3'));
% addpath(fullfile(A_CD,'Playgrounds'));

warning off all
warning off Simulink:blocks:TDelayTimeTooSmall
warning off Simulink:Engine:SaveWithDisabledLinks_Warning
warning off Simulink:Engine:LineWithoutDst