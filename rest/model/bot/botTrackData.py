import model.bot.replaymetadata

class BotTrackData:

    is_earrape = False
    is_loop = False

    # when this element is first in a list
    # the currently playing item will be skipped
    # -> this can lead to instant skipping an entire list
    #     if all titles are tagget ForceReplay
    # -> should only be set for instantButtons etc.
    force_replay = False


    # foreign key to TrackData
    track_id = 0
    