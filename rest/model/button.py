from model.trackData import TrackData

class Button:
 
    id = None

    position = 0
    nick_name = ''

    is_earrape = False
    is_loop = False

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
        self.position = db_tuple[1]
        self.nick_name = db_tuple[2]
        self.is_earrape = db_tuple[3]
        self.is_loop = db_tuple[4]
        self.track_id = db_tuple[5]



    def parse_from_json(self, db_dict):
        self.id = db_dict.get('id', None)
        self.position = db_dict.get('position', 0)
        self.nick_name = db_dict.get('nick_name', '')
        self.is_earrape = db_dict.get('is_earrape', False)
        self.is_loop = db_dict.get('is_loop', False)
        self.track_id = db_dict.get('track_id', None)

        if 'track' in db_dict:
            self.track = TrackData(db_dict['track'])




    def to_json(self):
        d = dict({
            'id': self.id,
            'position': self.position,
            'nick_name': self.nick_name,
            'is_earrape': self.is_earrape,
            'is_loop': self.is_loop,
            'track_id': self.track_id
        })


        # foreign key
        if self.track is not None:
            d['track'] = self.track.to_json()

        return d



