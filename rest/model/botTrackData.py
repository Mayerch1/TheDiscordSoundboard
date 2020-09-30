from model.trackData import TrackData

class BotTrackData:

    id = None
    is_earrape = False
    is_loop = False

    # when this element is first in a list
    # the currently playing item will be skipped
    # -> this can lead to instant skipping an entire list
    #     if all titles are tagget ForceReplay
    # -> should only be set for instantButtons etc.
    force_replay = False


    # foreign key to TrackData
    track_id = None
    track = None


    def __init__(self, raw_input = None):

        # input from database
        if isinstance(raw_input, tuple):
            self.parse_from_tuple(raw_input)

        # input from rest
        elif isinstance(raw_input, dict):
            self.parse_from_json(raw_input)
            pass



    def parse_from_tuple(self, db_tuple):
        self.id = db_tuple[0]
        self.is_earrape = db_tuple[1]
        self.is_loop = db_tuple[2]
        self.force_replay = db_tuple[3]
        self.track_id = db_tuple[4]


    def parse_from_json(self, db_dict):
        self.id = db_dict.get('id', None)
        self.is_earrape = db_dict.get('is_earrape', 0)
        self.is_loop = db_dict.get('is_loop', '')
        self.force_replay = db_dict.get('force_replay', False)
        self.track_id = db_dict.get('track_id', None)

        if 'track' in db_dict:
            self.track = TrackData(db_dict['track'])




    def to_json(self):
        d = dict({
            'id': self.id,
            'is_earrape': self.is_earrape,
            'is_loop': self.is_loop,
            'force_replay': self.force_replay,
            'track_id': self.track_id
        })


        # foreign key
        if self.track is not None:
            d['track'] = self.track.to_json()

        return d
