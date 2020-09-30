from model.trackData import TrackData
from model.botTrackData import BotTrackData

class BotState:
    # botState is volatile, no database relation
    # id is only in for potential transfer to db
    id = None

    # this is actively played by the bot
    current_track = None
    
    # list of BotTrackData
    queue = []


    def __init__(self, raw_input = None):

        # input from rest
        if isinstance(raw_input, dict):
            self.parse_from_json(raw_input)



    def parse_from_json(self, db_dict):
        self.id = db_dict.get('id', None)

        if 'current_track' in db_dict:
            self.current_track = BotTrackData(db_dict['current_track'])

        # parse the entire queue
        if 'queue' in db_dict:
            #TODO implement json to list
            pass



    def to_json(self):
        d = dict({
            'id': self.id
        })


        # foreign key
        if self.current_track is not None:
            d['current_track'] = self.current_track.to_json()


        # parse the list to json
        if self.queue is not None:
            queue_json = []
            [queue_json.append(trk.to_json()) for trk in self.queue]
            d['queue'] = queue_json


        return d