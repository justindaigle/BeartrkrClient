namespace BeartrkrClient
{
    class ProcessItem
    {
        public string Name { get; private set; }
        public ulong Time { get; private set; }
        public bool Web { get; private set; }

        public ProcessItem(string name, ulong time)
        {
            Name = name;
            Time = time;
            Web = false;
        }

        public ProcessItem(string name, ulong time, bool web)
        {
            Name = name;
            Time = time;
            Web = web;
        }

        public void IncTime()
        {
            Time++;
        }

        public void ClearTime()
        {
            Time = 0;
        }

        public void SetWebTime()
        {
            Time = 60;
        }
    }
}
