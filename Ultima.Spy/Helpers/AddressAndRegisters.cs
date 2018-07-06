using System;

namespace Ultima.Spy
{
    /// <summary>
    /// Processor registers.
    /// </summary>
    public enum Register : int
    {
        Eax = 1,
        Ebx = 2,
        Ecx = 3,
        Edx = 4,
        Esi = 5,
        Edi = 6,
        Ebp = 7,
        Esp = 8
    }

    /// <summary>
    /// Describes client address and register.
    /// </summary>
    public class AddressAndRegisters
    {
        #region Properties

        /// <summary>
        /// Gets or sets client address.
        /// </summary>
        public uint Address { get; set; }


        /// <summary>
        /// Gets or sets data address register.
        /// </summary>
        public Register DataAddressRegister { get; set; }

        /// <summary>
        /// Gets or sets data length register.
        /// </summary>
        public Register DataLengthRegister { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new instance of AddressAndRegisters.
        /// </summary>
        /// <param name="address">Client address.</param>
        /// <param name="dataAddressRegister">Data address register.</param>
        /// <param name="dataLengthRegister">Data length register.</param>
        public AddressAndRegisters(int address, Register dataAddressRegister, Register dataLengthRegister)
        {
            Address = (uint)address;
            DataAddressRegister = dataAddressRegister;
            DataLengthRegister = dataLengthRegister;
        }

        /// <summary>
        /// Constructs a new instance of AddressAndRegisters.
        /// </summary>
        /// <param name="address">Client address.</param>
        /// <param name="dataAddressRegister">Data address register.</param>
        /// <param name="dataLengthRegister">Data length register.</param>
        public AddressAndRegisters(int address, int dataAddressRegister, int dataLengthRegister)
        {
            Address = (uint)address;
            DataAddressRegister = (Register)dataAddressRegister;
            DataLengthRegister = (Register)dataLengthRegister;
        }
        #endregion
    }
}