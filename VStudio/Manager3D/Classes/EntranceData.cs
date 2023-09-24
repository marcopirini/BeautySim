using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Manager3D
{
    using System;
    using System.ComponentModel;
    using System.Windows.Media.Media3D;

    public class EntranceData : INotifyPropertyChanged
    {
        private bool _toBeClosed = true;
        public bool ToBeClosed
        {
            get { return _toBeClosed; }
            set
            {
                _toBeClosed = value;
                NotifyPropertyChanged(nameof(ToBeClosed));
            }
        }

        private string _structureEntered;
        public string StructureEntered
        {
            get { return _structureEntered; }
            set
            {
                _structureEntered = value;
                NotifyPropertyChanged(nameof(StructureEntered));
            }
        }

        private Point3D _collisionPoint;
        public Point3D CollisionPoint
        {
            get { return _collisionPoint; }
            set
            {
                _collisionPoint = value;
                NotifyPropertyChanged(nameof(CollisionPoint));
            }
        }

        private double _entryingPointYaw;
        public double EntryingPointYaw
        {
            get { return _entryingPointYaw; }
            set
            {
                _entryingPointYaw = value;
                NotifyPropertyChanged(nameof(EntryingPointYaw));
            }
        }

        private double _entryingPointPitch;
        public double EntryingPointPitch
        {
            get { return _entryingPointPitch; }
            set
            {
                _entryingPointPitch = value;
                NotifyPropertyChanged(nameof(EntryingPointPitch));
            }
        }

        private DateTime _entranceTime;
        public DateTime EntranceTime
        {
            get { return _entranceTime; }
            set
            {
                _entranceTime = value;
                NotifyPropertyChanged(nameof(EntranceTime));
            }
        }

        private DateTime _exitingTime;
        public DateTime ExitingTime
        {
            get { return _exitingTime; }
            set
            {
                _exitingTime = value;
                NotifyPropertyChanged(nameof(ExitingTime));
            }
        }

        private int _index;


        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                NotifyPropertyChanged(nameof(Index));
            }
        }

        private double _depth;

        public double Depth
        {
            get { return _depth; }
            set
            {
                _depth = value;
                NotifyPropertyChanged(nameof(Depth));
            }
        }



        public EntranceData(string structureEntered, Point3D collisionPoint, double entryingPointYaw, double entryingPointPitch)
        {
            StructureEntered = structureEntered;
            CollisionPoint = collisionPoint;
            EntryingPointYaw = entryingPointYaw;
            EntryingPointPitch = entryingPointPitch;
            EntranceTime = DateTime.Now;
        }

        public EntranceData()
        {
            StructureEntered = "";
            CollisionPoint = new Point3D();
            EntryingPointYaw = 0;
            EntryingPointPitch = 0;
            EntranceTime = DateTime.Now;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal string ToReadableString()
        {
            return $"{StructureEntered} {CollisionPoint} {EntryingPointYaw} {EntryingPointPitch} {EntranceTime} {ExitingTime}";
        }
    }

}
