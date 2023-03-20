%% A4_1_Statistics
% Inputs        Parameters, Simulation, Disturbance?, OutData, OutList
%               PP.Stat(i).Signal
%               PP.Stat(i).Duration

% Ouputs        PP.Stat(i).Mean
%                         .Max
%                         .Min
%                         .Std

N = length(PP.Stat);

for n = 1:N
    dat_ind = strmatch(PP.Stat(n).Signal,OutList,'exact');
    if isempty(dat_ind)
        disp([PP.Stat(n).Signal,' isnt in OutList']);
    elseif PP.Stat(n).Duration > Simulation.TMax
        disp('Steady state data duration greater than sim. time');
    else
        %Time-Domain Data
        time_ind    = PP.Stat(n).Duration/DT;
        
        if max(OutData.time) > PP.Stat(n).Duration
            dat         = OutData.signals.values(end-time_ind:end,dat_ind);
            time        = OutData.time(end-time_ind:end);
            
            %Statistics
            PP.Stat(n).Mean = mean(dat);
            PP.Stat(n).Min  = min(dat);
            PP.Stat(n).Std  = std(dat);
            PP.Stat(n).Max  = max(dat);
        else
            %Statistics
            disp('ERROR: Simulation time < Stat. Time');
            PP.Stat(n).Mean = NaN;
            PP.Stat(n).Min  = NaN;
            PP.Stat(n).Std  = NaN;
            PP.Stat(n).Max  = NaN;
        end
        
        
    end
    
end
