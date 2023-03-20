%% Plot Channels
% This is a terrible script, should be commented

TIGHT = 0;  %create tighter subplots for presentations, etc.
PLOT = 1;   %to hide plot if 0

nPlots = length(PP.Channels);
h = cell(1,nPlots);
Chan = struct;
Chan.tt = OutData.signals.values(:,1);
for iPlot = 1:nPlots
    nChannels = length(PP.Channels{iPlot});
    if TIGHT && PLOT
        figure(iPlot);
        if ~PP.TimePlot.hold || isempty(get(gcf,'Children'))
            h{iPlot}=tight_subplot(nChannels,1,.03,.05,.05);
        else
            h{iPlot} = flipud(get(gcf,'Children'));
        end
    end
    for iChannel = 1:nChannels
        if PLOT
            if ~TIGHT
                figure(iPlot); subplot(nChannels,1,iChannel);
            else
                axes(h{iPlot}(iChannel));
            end
        end
        
        %% Collect Data & Plot
        dat_ind = strmatch(PP.Channels{iPlot}{iChannel}{1},OutList,'exact');
        if isempty(dat_ind) && isempty(PP.Channels{iPlot}{iChannel}{2})
            disp([PP.Channels{iPlot}{iChannel},' isnt in OutList']);
        else
            if isempty(PP.Channels{iPlot}{iChannel}{2})
                dat = OutData.signals.values(:,dat_ind);
                eval(['Chan.',PP.Channels{iPlot}{iChannel}{1},'=','dat;'])
            else
                try
                    dat = eval(PP.Channels{iPlot}{iChannel}{2});
                    eval(['Chan.',PP.Channels{iPlot}{iChannel}{1},'=','dat;'])
                catch err
                    disp('Couldn''t execute eval, see err');
                    eval(['Chan.',PP.Channels{iPlot}{iChannel}{1},'=','dat;'])
                end
            end
            
            %% Do Plotting
            if PLOT
                %             plot(tt,dat);
                plot(Chan.tt,dat,'LineWidth',2);
                if TIGHT
                    axis tight;
                else
                    ylabel(PP.Channels{iPlot}{iChannel}{1});
                end
                if iChannel ~= nChannels
                    set(gca,'XTickLabel',{})
                end
                %             grid on;
                if ~isempty('PP.TimePlot.Xlim'), xlim(PP.TimePlot.Xlim); end
                if PP.TimePlot.hold, hold on; end
            end
        end
        
    end %iChannel
    if PLOT
        xlabel('Time [s]');
    end
end %iPlot
